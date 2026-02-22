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


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitializeGame();
    }

    // 게임 초기화
    private void InitializeGame()
    {
        //테스트용 P1, P2
        var player1 = new Player("P1");
        var player2 = new Player("P2");

        playersById = new Dictionary<string, Player>
        {
            { player1.PlayerId, player1 },
            { player2.PlayerId, player2 }
        };

        CreateToken(player1, "T1");
        CreateToken(player2, "T2");

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

    }

 

}
