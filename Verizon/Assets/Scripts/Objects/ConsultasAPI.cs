using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using System.Text;

[System.Serializable]
public class ConsultasAPI : MonoBehaviour 
{

	// Direccion de la API para las consultas
	public string addressAPI;

	// Eventos de caso de exito, evento vacio y error
	public UnityEvent succesEvent;
	public UnityEvent emptyEvent;
	public UnityEvent errorEvent;

	// Elementos para comparar
	public List<ListaElementos> postElements;
    public List<ListaElementos> headersClass;
    private Dictionary<string, string> headers;

    public string[] succesConditions;
	public string[] emptyConditions;
	public string[] errorConditions;

	

	// Variables para eventos fuera de esta clase

	public string result;

    public Texture2D texturaResultado;
    public WWW wwwImagen;

    public delegate void FinishSuccesDelegate(string resultMessage);
    public FinishSuccesDelegate finishSucces;

    public delegate void FinishEmptyDelegate(string resultMessage);
    public FinishEmptyDelegate finishEmpty;

    public delegate void FinishErrorDelegate(string resultMessage);
    public FinishErrorDelegate finishError;




    /// <summary>
    /// Agrega headers para las consultas del Web Service
    /// </summary>
    /// <param name="Llave">Nombre de la variables</param>
    /// <param name="Contenido">Valor de la llave.</param>
    public void AddHeader(string key, string content)
	{
        headersClass.Add(new ListaElementos(key, content));
	}

	/// <summary>
	/// Reemplaza el valor de una cabecera especificada
	/// </summary>
	/// <param name="Llave">Llave a buscar.</param>
	/// <param name="Contenido">Nuevo valor a reemplazar.</param>
	public void ReplaceHeader(string key, string content)
	{
        for (int i = 0; i < headersClass.Count; i++)
        {
            if (headersClass[i].keyName.Equals(key))
            {
                headersClass[i].stringField = content;
                headersClass[i].compareType = ListaElementos.FormasComparacion.stringEnum;
                break;
            }
        }
	}

	public void Get ()
	{
		StartCoroutine(RoutineConsult());
	}

    public void Post()
    {
        StartCoroutine(RoutineConsult());
    }

    public void Post(byte[] body)
    {

    }

	public IEnumerator RoutineConsult(object parameter = null)
	{
        WWWForm form = new WWWForm();
        byte[] bytesData = null;

        if (parameter != null)
        {
            Type type = parameter.GetType();
            if (type.Equals( typeof(byte[]) ))
                bytesData = (byte[])parameter;
            else if (type.Equals(typeof(string)))
                bytesData = System.Text.Encoding.UTF8.GetBytes((string)parameter);     
        }

        #region HeaderBuilder
        if (headers == null)
            headers = new Dictionary<string, string>();
        else
            headers.Clear();

        if (headersClass.Count > 0)
        {
            foreach (ListaElementos element in headersClass)
            {
                string field = "";
                switch(element.compareType)
                {
                    case ListaElementos.FormasComparacion.floatEnum:
                        field = element.floatField.ToString();
                    break;

                    case ListaElementos.FormasComparacion.inputFieldEnum:
                        field = element.inputField.text;
                    break;

                    case ListaElementos.FormasComparacion.intEnum:
                        field = element.integerField.ToString();
                    break;

                    case ListaElementos.FormasComparacion.stringEnum:
                        field = element.stringField;
                    break;

                    case ListaElementos.FormasComparacion.textEnum:
                        field = element.textField.text;
                    break;

                    case ListaElementos.FormasComparacion.toggleEnum:
                        field = element.toggleField.isOn ? "true" : "false";
                    break;
                }
                headers.Add(element.keyName, field);
            }
        }
        #endregion HeaderBuilder

        #region PostBuilder
        string postString = "{\n";
        if (postElements.Count > 0)
        {
            foreach (ListaElementos element in postElements)
            {
                string field = "";
                switch (element.compareType)
                {
                    case ListaElementos.FormasComparacion.floatEnum:
                        field = element.floatField.ToString();
                        break;

                    case ListaElementos.FormasComparacion.inputFieldEnum:
                        field = element.inputField.text;
                        break;

                    case ListaElementos.FormasComparacion.intEnum:
                        field = element.integerField.ToString();
                        break;

                    case ListaElementos.FormasComparacion.stringEnum:
                        field = element.stringField;
                        break;

                    case ListaElementos.FormasComparacion.textEnum:
                        field = element.textField.text;
                        break;

                    case ListaElementos.FormasComparacion.toggleEnum:
                        field = element.toggleField.isOn ? "true" : "false";
                        break;
                }

                form.AddField(element.keyName, field);
                postString = postString + "\t\"" + element.keyName + "\"\t: " + field + ",\n";
            }
            postString = postString+ "}";
            postString = postString.Replace(",\n}", "\n}");
            bytesData = System.Text.Encoding.UTF8.GetBytes(postString);
            Debug.Log(this.name + " post info: " + postString);
        }
        #endregion PostBuilder

        WWW www = new WWW(addressAPI, bytesData,  headers );
		yield return www;

        #region QueryResult
        if (www.error == null)
		{
			Debug.Log(this.name + " result " + www.text);
			result = www.text;

			for (int i = 0; i < errorConditions.Length; i++)
			{
				if ( www.text.Contains (errorConditions[i] ) )
				{
                    Debug.Log(this.name + " error porque tiene " + errorConditions[i]);
					errorEvent.Invoke();
                    finishError(www.text);
                    yield break;
				}
			}

			for (int i = 0; i < emptyConditions.Length; i++)
			{
				if ( www.text.Contains (emptyConditions[i] ) )
				{
					Debug.Log(this.name + " vacio porque tiene " + emptyConditions[i]);
					emptyEvent.Invoke ();
                    finishEmpty(www.text);
                    yield break;
				}
			}
				


			if (succesConditions.Length == 0)
			{
				succesEvent.Invoke();
                if (finishSucces!=null)
                    finishSucces(www.text);
			}
			else
			{
				for (int i = 0; i < succesConditions.Length; i++)
				{
					
					if ( www.text.Contains (succesConditions[i] ) )
					{
						succesEvent.Invoke();
                        finishSucces(www.text);
                        break;
					}
				}
			}
		}
		else
		{
			Debug.Log("<Color=RED>" + this.name + www.error + "</Color>");
			result = www.error;
			errorEvent.Invoke();
		}
        #endregion QueryResult
    }

    /*
	public IEnumerator RutinaConsultaHeadersPost()
	{
		WWWForm form = new WWWForm();


		Dictionary<string,string> headers = new Dictionary<string,string>();

		for (int i = 0; i < headers.Count; i++)
		{
			headers.Add(headers[i], Valores[i]);

		}

		string CadenaConsulta = "{\n";

		if (Elementos.Count > 0)
		{
			foreach(ListaElementos listelem in Elementos)
			{
				string Campo = (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.Float)) ? 
					listelem.Flotante.ToString() : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.InputField)) ? 
					"\"" + listelem.CadenaEntrada.text + "\"": (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.Int)) ? 
					( (listelem.Entero < 0) ? "null" : listelem.Entero.ToString() ): (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.String)) ? 
					"\"" + listelem.Cadena + "\"" : (listelem.TiposComparacion.Equals(ListaElementos.FormasComparacion.Toggle)) ?
					( (listelem.Togleador.isOn) ? "1" : "0"  ) : "\"" + listelem.TextoFijo.text + "\"";

				form.AddField(listelem.NombreCampo, Campo);
				CadenaConsulta = CadenaConsulta + "\t\"" + listelem.NombreCampo + "\"\t: " + Campo + ",\n";
			}

			CadenaConsulta = CadenaConsulta + "}";
			CadenaConsulta = CadenaConsulta.Replace(",\n}","\n}");
			Debug.Log(this.name + CadenaConsulta);


		}

		var DatosPars = System.Text.Encoding.UTF8.GetBytes(CadenaConsulta);

		WWW www = new WWW(DireccionAPI, DatosPars, headers);
		yield return www;

		if (www.error == null)
		{
			Debug.Log(this.name + www.text);

			Resultado = www.text;

			for (int i = 0; i < CasosError.Length; i++)
			{
				if ( www.text.Contains ( CasosError [i] ) )
				{
					CasoDeError = i;
					EventoError.Invoke();
					//yield return null;
					goto salida;
				}
			}

			for (int i = 0; i < CasosVacio.Length; i++)
			{
				if ( www.text.Contains ( CasosVacio [i] ) )
				{
                    EventoVacio.Invoke ();
					goto salida;
				}
			}


            if (CasosExito.Length == 0)
			{
				EventoExito.Invoke();
			}
			else
			{
				for (int i = 0; i < CasosExito.Length; i++)
				{
					if ( www.text.Contains ( CasosExito [i] ) )
					{
						EventoExito.Invoke();
						break;
					}
				}
			}
		}
		else
		{
			Debug.Log(this.name + www.error);
			Resultado = www.error;

			EventoError.Invoke();
		}

		salida:
		Debug.Log("Salio de la funcion");

	}
    */
}

