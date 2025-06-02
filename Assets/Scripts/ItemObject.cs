using TMPro;
using UnityEngine;
using Photon.Pun;
using System.Threading;

public class ItemObject : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public ItemInstance Data;
    private PhotonView view;

    private float Timer = 0f;

    void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {

        string json = (string)PhotonView.Get(this).InstantiationData[0];
        int itemID = (int)PhotonView.Get(this).InstantiationData[1];

        transform.SetParent(Game.g_items.transform);
        Data = new ItemInstance(JsonUtility.FromJson<ItemInstanceSender>(json));

        Transform model = transform.Find("Model");
        if (model.childCount == 0)
        {
            GameObject newModel = Instantiate(Data.itemRef.model, model);

            newModel.transform.localPosition = Vector3.zero;
        }
        transform.Find("Canvas").Find("Name").GetComponent<TextMeshProUGUI>().SetText($"x{Data.amount} {Data.itemRef.itemName}");

        transform.gameObject.name = $"{itemID}";
    }

    private void Update()
    {
        Timer += Time.fixedDeltaTime;

        if (PhotonNetwork.IsMasterClient)
        {
            if (Timer > DayNightCycle2D.DayDuration * 2)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
