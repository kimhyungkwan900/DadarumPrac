using UnityEngine;

public class ChoiceManager : MonoBehaviour
{
    public static ChoiceManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ExecuteChoice(ChoiceData choice)
    {
        if (choice == null) return;
        if (choice.effects == null) return;

        foreach ( var effect in choice.effects){
            effect?.Apply();
        }
    }
}
