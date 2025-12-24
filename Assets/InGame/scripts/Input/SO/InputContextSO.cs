using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Input/Context Policy")]
public class InputContextPolicy : ScriptableObject
{
    public InputContext context;
    public List<ActionId> allowedActions;
}
