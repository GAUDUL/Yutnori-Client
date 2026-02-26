using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject tokenPrefab;
    [SerializeField] private BoardView boardView;
    [SerializeField] private StepSelectionUI stepSelectionUI;
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
            string playerId = $"P{i}";
            var player = new Player(playerId);
            playersById.Add(playerId, player);

            // 말 2개 생성
            CreateToken(player, $"{playerId}_T1");
            CreateToken(player, $"{playerId}_T2");
        }

        gameCore = new GameCore(BOARD_SIZE, playersById, tokens);

        // 초기 UI 상태 동기화
        RefreshStepUI();
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
    
    // UI 동기화
    private void RefreshStepUI()
    {
        var steps = new List<int>(gameCore.GetPendingSteps());
        bool selectable =
            gameCore.CanSelectStep &&
            selectedTokenId != null;

        stepSelectionUI.Show(steps, selectable, OnStepSelected);
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

        Debug.Log($"[Test] {gameCore.CurrentTurnPlayerId} 윷 던지기 결과: ({rollResult.ResultStep}칸)");

        RefreshStepUI();
    }


    //말 선택 (Token 선택 시 실행)
    public void OnSelectToken(string tokenId)
    {
        bool success = gameCore.SelectToken(tokenId);

        if (!success)
        {
            Debug.Log("말 선택 불가 상태");
            return;
        }

        selectedTokenId = tokenId;

        RefreshStepUI();
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

        UpdateTokenView(moveResult);
        HandleMoveResult(moveResult);

        selectedTokenId = null;
        selectedStep = null;

        RefreshStepUI();
    }

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
        string currentTurnPlayerId = moveResult.CurrentTurnPlayerId;

        if (!captured)
        {
            Debug.Log($"[Test] 턴 종료. 다음 플레이어: {currentTurnPlayerId}");
        }
        else
        {
            Debug.Log($"[Test] {moveResult.GroupId} 잡기 성공, {currentTurnPlayerId} 추가 턴 부여");
        }

        if (moveResult.IsRoundEnd)
        {
            Debug.Log("[Test] 라운드 종료");

            foreach (var player in playersById.Values)
            {
                Debug.Log($"남은 코인: {player.PlayerId} = {player.Coin}");
            }
        }
    }

}
