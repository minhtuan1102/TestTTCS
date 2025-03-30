using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;

public class BoardManager : MonoBehaviourPunCallbacks
{
    public TileBase[] GroundTiles;  // Mảng các tile sàn
    public TileBase[] WallTiles;    // Mảng các tile tường
    public int Width = 10;          // Chiều rộng bản đồ
    public int Height = 10;         // Chiều cao bản đồ
    private Tilemap m_Tilemap;

    // Bỏ Start() vì không cần kiểm tra ở đây nữa

    public override void OnJoinedRoom()
    {
        Debug.Log("BoardManager OnJoinedRoom - IsMasterClient: " + PhotonNetwork.IsMasterClient);
        if (PhotonNetwork.IsMasterClient)
        {
            Init();
        }
    }

    [PunRPC]
    public void SyncTile(int x, int y, int tileIndex, bool isWall)
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        TileBase tile = isWall ? WallTiles[tileIndex] : GroundTiles[tileIndex];
        m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
    }

    void Init()
    {
        Debug.Log("BoardManager Init - Creating map");
        m_Tilemap = GetComponentInChildren<Tilemap>();
        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                TileBase tile;
                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1) // Tường viền ngoài
                {
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    photonView.RPC("SyncTile", RpcTarget.All, x, y, System.Array.IndexOf(WallTiles, tile), true);
                }
                else // Sàn bên trong
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    photonView.RPC("SyncTile", RpcTarget.All, x, y, System.Array.IndexOf(GroundTiles, tile), false);
                }
            }
        }
    }
}