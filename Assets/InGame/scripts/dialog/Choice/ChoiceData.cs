using System.Collections.Generic;
using UnityEngine;

public class Choice
{
    public string text;
    public List<IChoiceCondition> conditions;
    public List<IChoiceEffect> effects;
}
public class ChoiceGroup
{
    public string id;
    public List<Choice> choices;
}
