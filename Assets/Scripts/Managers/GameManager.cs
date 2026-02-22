using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject tokenPrefab;
    [SerializeField] private BoardView boardView;
    [SerializeField] private float tokenSpawnHeight = 1.0f;

    private const int BOARD_SIZE = 20;

    private Dictionary<string, Player> playersById;
    private List<Token> tokens = new List<Token>();
    private Dictionary<string, TokenView> tokenViews = new Dictionary<string, TokenView>();

    private GameCore gameCore;

    private Player localPlayer;
    private string localPlayerId = "P1";

    public bool IsRoundEnd;


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
    }

    //말 생성
    private void CreateToken(Player player, string tokenId)
    {
        var token = new Token(player.PlayerId, tokenId);
        tokens.Add(token);

        GameObject obj = Instantiate(tokenPrefab);
        var view = obj.GetComponent<TokenView>();
        view.Initialize(tokenId);
        tokenViews[tokenId] = view;

        Vector3 basePos = boardView.GetWorldPosition(token.CurrentTileIndex);
        obj.transform.position = basePos + new Vector3(0, tokenSpawnHeight, 0);
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
             
    }

    //말 선택 후 이동
    public void OnSelectToken(string tokenId)
    {
        MoveResult moveResult = gameCore.Move(tokenId);

        if (!moveResult.IsSuccess)
        {
            Debug.Log($"{moveResult.Error}");
            return;
        }

        int index = moveResult.NewIndex;

        tokenViews[moveResult.TokenId].transform.position = boardView.GetWorldPosition(index) + new Vector3(0, tokenSpawnHeight, 0);
        Debug.Log($"[Test] {moveResult.TokenId} 이동 완료/ 현재 타일: {index}");

        bool captured = moveResult.Captured;
        string currentTurnPlayerId = moveResult.CurrentTurnPlayerId;

        if (!captured)
        {
            Debug.Log($"[Test] 턴 종료. 다음 플레이어: {currentTurnPlayerId}");
        }
        else
        {
            //추가 턴
            Debug.Log($"[Test] {moveResult.TokenId} 잡기 성공, {currentTurnPlayerId} 추가 턴 부여");
        }

        if (moveResult.IsRoundEnd)
        {
            Debug.Log("[Test] 라운드 종료");

            foreach (var player in playersById.Values)
            {
                Debug.Log($"남은 코인: {player.PlayerId} = {player.Coin}");
            }

            // 미니게임 이동
        }

    }

 

}
