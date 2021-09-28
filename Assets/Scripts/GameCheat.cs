using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameCheat : MonoBehaviour
{
    [SerializeField] TMP_InputField _inputField;
    void Start()
    {
        _inputField.onEndEdit.AddListener(fieldValue =>
        {
            if (Input.GetKeyDown(KeyCode.Return) | Input.GetKeyDown(KeyCode.KeypadEnter))
                ProcessCommand(fieldValue);
        });

    }
    public bool IsEnable 
    {
        get 
        {
            return _inputField.gameObject.activeInHierarchy;
        }
    }

    List<int> numbers = new List<int>();
    int _currentPickUp;
    void ProcessCommand(string x)
    {
        numbers = ParseDiceNumber();
        _currentPickUp = 0;
    }

    public List<int> ParseDiceNumber()
    {
        string inputText = _inputField.text;
        string[] numbersTxt = inputText.Split(',');
        List<int> inputtedNumbers = new List<int>();
        foreach (var item in numbersTxt)
            inputtedNumbers.Add(int.Parse(item));

        return inputtedNumbers;
    }

    public int GetNextUserSpecifiedDiceNumber()
    {
        int toDisplay = _currentPickUp;
        _currentPickUp++;

        if (toDisplay < numbers.Count)
            return numbers[toDisplay];
        else
            return 0;
    }

    void Update()
    {
        
    }
}
