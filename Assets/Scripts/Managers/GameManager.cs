using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject tokenPrefab;
    [SerializeField] private BoardView boardView;

    [SerializeField] private StepSelectionUI stepSelectionUI;
    [SerializeField] private MergeSelectionUI mergeSelectionUI;
    [SerializeField] private ItemSelectionUI itemSelectionUI;
    [SerializeField] private PlayerSelectionUI playerSelectionUI;

    [SerializeField] private float tokenSpawnHeight = 1.0f;

    private const int BOARD_SIZE = 20;

    private Dictionary<string, Player> playersById;
    private List<Token> tokens = new List<Token>();
    private Dictionary<string, TokenView> tokenViews = new Dictionary<string, TokenView>();

    private GameCore gameCore;

    private Player localPlayer;
    private string localPlayerId = "P1";

    public bool IsRoundEnd;

    private string selectedTokenId;
    private int? selectedStep;

    private string selectedItemId;
    private ItemTargetType selectedItemTargetType;
    private bool isSelectingItemTarget;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //테스트 용 플레이어 수
        int testPlayerCount = 2;
        InitializeGame(testPlayerCount);
    }

    // 게임 초기화
    private void InitializeGame(int playerCount)
    {
        playersById = new Dictionary<string, Player>();

        //플레이어 생성
        for (int i = 1; i <= playerCount; i++)
        {
            string playerId = Guid.NewGuid().ToString();
            string playerName = $"P{i}";
            var player = new Player(playerId, playerName);
            playersById.Add(playerId, player);

            // 말 2개 생성
            CreateToken(player, $"{playerId}_T1");
            CreateToken(player, $"{playerId}_T2");
        }

        gameCore = new GameCore(BOARD_SIZE, playersById, tokens);

        boardView.ApplyAllTiles(gameCore.GetTiles());


        stepSelectionUI.Hide();
        itemSelectionUI.Hide();
        playerSelectionUI.Hide();

        // 초기 UI 상태 동기화
        RefreshStepUI();
        RefreshItemUI();
    }

    //말 생성
    private void CreateToken(Player player, string tokenId)
    {
        int startTileIndex = 0;

        var token = new Token(player.PlayerId, tokenId);
        tokens.Add(token);

        GameObject obj = Instantiate(tokenPrefab);
        var view = obj.GetComponent<TokenView>();
        view.Initialize(tokenId);
        tokenViews[tokenId] = view;

        Vector3 basePos = boardView.GetWorldPosition(startTileIndex);
        obj.transform.position = basePos + new Vector3(0, tokenSpawnHeight, 0);
    }
    
    // Step UI 동기화
    private void RefreshStepUI()
    {
        if (gameCore.CanSelectStep)
        {
            var steps = new List<int>(gameCore.GetPendingSteps());
            bool selectable = selectedTokenId != null;

            stepSelectionUI.Show(steps, selectable, OnStepSelected);
        }
        else
        {
            stepSelectionUI.Hide();
        }
    }
    
    // Item UI 동기화
    private void RefreshItemUI()
    {
        string playerId = gameCore.CurrentTurnPlayerId;

        if (gameCore.CanUseItem)
        {
            var items = new List<Item>(playersById[playerId].Items);

            itemSelectionUI.Show(items, OnItemSelected);
        }
        else
        {
            itemSelectionUI.Hide();
        }
    }

    //윷 던지기
    public void OnClickThrowButton()
    {
        RollResult rollResult = gameCore.Roll(localPlayerId);
        if (!rollResult.IsSuccess)
        {
            Debug.Log($"{rollResult.Error}");
            return;
        }

        Debug.Log($"[Test] {gameCore.CurrentTurnPlaeyrName} 윷 던지기 결과: ({rollResult.ResultStep}칸)");

        RefreshStepUI();
        RefreshItemUI();
    }


    //말 선택 (Token 선택 시 실행)
    public void OnSelectToken(string tokenId)
    {

        if (isSelectingItemTarget)
        {
            HandleItemTokenTarget(tokenId);
            return;
        }

        bool success = gameCore.SelectToken(tokenId);

        if (!success)
        {
            Debug.Log("말 선택 불가 상태");
            return;
        }

        selectedTokenId = tokenId;

        RefreshStepUI();
        RefreshItemUI();
    }

    // 선택한 말 이동 (Step 선택 시 실행)
    private void OnStepSelected(int step)
    {
        selectedStep = step;

        if (selectedTokenId == null)
            return;

        MoveResult moveResult = gameCore.Move(selectedStep.Value);

        if (!moveResult.IsSuccess)
        {
            Debug.Log(moveResult.Error);
            return;
        }
        
        var tiles = moveResult.TileEffectResult?.ChangedTiles;

        if (tiles != null)
        {
            if (moveResult.TileEffectResult?.IsConnected == true)
            {
                Debug.Log($"{tiles[0].tileIndex} & {tiles[1].tileIndex} 타일 연결");
            }
            else
            {
                boardView.ApplySomeTiles(moveResult.TileEffectResult.ChangedTiles);
            }
        }

        UpdateTokenView(moveResult);
        HandleMoveResult(moveResult);

        selectedTokenId = null;
        selectedStep = null;

        RefreshStepUI();
        RefreshItemUI();
    }

    //말 이동 view
    private void UpdateTokenView(MoveResult moveResult)
    {
        int index = moveResult.NewIndex;

        foreach (var tokenId in moveResult.MovedTokenIds)
        {
            tokenViews[tokenId].transform.position =
            boardView.GetWorldPosition(index) +
            new Vector3(0, tokenSpawnHeight, 0);
        }

        Debug.Log($"[Test] {moveResult.GroupId} 이동 완료 / 현재 타일: {index}");

    }

    private void HandleMoveResult(MoveResult moveResult)
    {
        bool captured = moveResult.Captured;
        bool needMerge = moveResult.NeedMerge;

        if (needMerge)
        {
            mergeSelectionUI.Show(merge =>
            {
                var result = gameCore.MergeSelected(merge);
                if (!result.IsSuccess)
                    return;

                if (result.IsRoundEnd)
                {
                    foreach (var player in playersById.Values)
                    {
                        Debug.Log($"남은 코인: {player.DisplayName} = {player.Coin}");
                    }
                    Debug.Log("[Test] 라운드 종료");
                }

                RefreshStepUI();
            });

            return;
        }

        if (!captured)
        {
            Debug.Log($"[Test] 턴 종료. 다음 플레이어: {gameCore.CurrentTurnPlaeyrName}");
        }
        else
        {
            Debug.Log($"[Test] {moveResult.GroupId} 잡기 성공, {gameCore.CurrentTurnPlaeyrName} 추가 턴 부여");
        }

        if (moveResult.IsRoundEnd)
        {
            Debug.Log("[Test] 라운드 종료");

            foreach (var player in playersById.Values)
            {
                Debug.Log($"남은 코인: {player.DisplayName} = {player.Coin}");
            }
        }
    }

    // 아이템 선택
    private void OnItemSelected(string itemId)
    {
        var player = playersById[gameCore.CurrentTurnPlayerId];
        var item = player.GetItemById(itemId);

        if (item == null)
        {
            Debug.Log("아이템 없음");
            return;
        }

        var effect = ItemEffects.Effects[item.Type];

        selectedItemId = itemId;
        selectedItemTargetType = effect.TargetType;

        isSelectingItemTarget = true;

        HandleItemTargetSelection();
    }

    // 아이템 타겟 설정
    private void HandleItemTargetSelection()
    {
        switch (selectedItemTargetType)
        {
            case ItemTargetType.None:
                ExecuteItem(null, null);
                break;

            case ItemTargetType.MyToken:
                Debug.Log("본인 말 선택 필요");
                break;

            case ItemTargetType.EnemyToken:
                Debug.Log("상대 말 선택 필요");
                break;

            case ItemTargetType.EnemyPlayer:
                playerSelectionUI.Show(GetEnemyPlayers(), OnPlayerSelected);
                break;

        }
    }

    // 타겟 토큰 선택
    private void HandleItemTokenTarget(string tokenId)
    {
        var token = tokens.Find(t => t.TokenId == tokenId);

        string targetPlayerId = token.PlayerId;

        if (selectedItemTargetType == ItemTargetType.MyToken &&
            targetPlayerId != gameCore.CurrentTurnPlayerId)
        {
            Debug.Log("본인 말만 선택 가능");
            return;
        }

        if (selectedItemTargetType == ItemTargetType.EnemyToken &&
            targetPlayerId == gameCore.CurrentTurnPlayerId)
        {
            Debug.Log("상대 말만 선택 가능");
            return;
        }

        ExecuteItem(targetPlayerId, tokenId);
    }

    // 아이템 실행
    private void ExecuteItem(string targetPlayerId, string targetTokenId)
    {
        var result = gameCore.UseItem(
            gameCore.CurrentTurnPlayerId,
            selectedItemId,
            targetPlayerId,
            targetTokenId
        );

        if (result.MoveResult != null)
        {
            UpdateTokenView(result.MoveResult);
        }

        if (!result.IsSuccess)
        {
            Debug.Log(result.Error);
            return;
        }

        selectedItemId = null;
        isSelectingItemTarget = false;

        RefreshItemUI();
    }

    // 타겟 플레이어 선택 (Coin Steal 아이템)
    private void OnPlayerSelected(string playerId)
    {
        ExecuteItem(playerId, null);
    }

    // 자신 제외 플레이어 리스트
    private List<Player> GetEnemyPlayers()
    {
        var list = new List<Player>();

        foreach (var p in playersById.Values)
        {
            if (p.PlayerId != gameCore.CurrentTurnPlayerId)
                list.Add(p);
        }

        return list;
    }

}
