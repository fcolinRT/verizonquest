using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class ListaElementos
{
	public int integerField;
	public float floatField;
	public string stringField;
	public InputField inputField;
	public Text textField;
	public Toggle toggleField;

    public string keyName;

    public enum FormasComparacion
    {
        intEnum,
        floatEnum,
        stringEnum,
        inputFieldEnum,
        textEnum,
        toggleEnum
    };
    public FormasComparacion compareType;

    public ListaElementos(string key, string value)
    {
        keyName = key;
        stringField = value;
        compareType = FormasComparacion.stringEnum;
    }

    

	


}


// A Partir de aqui editar segun JSON
[System.Serializable]
public class Response
{
    public string operation;
    public string code;
    public string message;
}








