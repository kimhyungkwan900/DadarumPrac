using UnityEngine;

// TODO Player 생성 로직 이동예정
public class GameManager : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Player가 없으면 생성
        if (GameObject.FindGameObjectWithTag("Player") == null && playerPrefab != null)
        {
            Instantiate(playerPrefab);
        }
    }
}
