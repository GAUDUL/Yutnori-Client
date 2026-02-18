using UnityEngine;
using UnityEngine.EventSystems;
public class TokenView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string tokenID;
    public string TokenID => tokenID;

    public void Initialize(string id)
    {
        tokenID = id;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.OnSelectToken(TokenID);
    }
}
