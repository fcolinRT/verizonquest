using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(ConsultasAPI))]
public class ConsultasAPIEditor : Editor 
{
	// Ruta de la API 
	SerializedProperty RutaAPI;
	// Lista reordenable de elementos para agregar
	ReorderableList ListaCondiciones;
    ReorderableList listaHeaders;

	SerializedProperty ListaCasosExito;
	SerializedProperty ListaCasosVacio;
	SerializedProperty ListaCasosError;

	SerializedProperty EventoBien;
	SerializedProperty EventoNul;
	SerializedProperty EventoMal;

	//SerializedProperty ListaValores;

	void OnEnable()
	{
		// Encuentra la propiedad de DireccionAPI
		RutaAPI = serializedObject.FindProperty ("addressAPI");
		EventoBien = serializedObject.FindProperty ("succesEvent");
		EventoNul = serializedObject.FindProperty ("emptyEvent");
		EventoMal = serializedObject.FindProperty ("errorEvent");

		ListaCasosExito = serializedObject.FindProperty("succesConditions");
		ListaCasosVacio = serializedObject.FindProperty("emptyConditions");
		ListaCasosError = serializedObject.FindProperty("errorConditions");


		// Inicia la condicion 
		ListaCondiciones = new ReorderableList ( serializedObject, serializedObject.FindProperty("postElements"), true, true, true, true );
		// Diseña como se dibuja cada elemento de la lista
		ListaCondiciones.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) => 
		{
			var elemento = ListaCondiciones.serializedProperty.GetArrayElementAtIndex(index);

			
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("keyName"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + (rect.width / 3), rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("compareType"), GUIContent.none);

            ListaElementos.FormasComparacion Tipo = (ListaElementos.FormasComparacion) elemento.FindPropertyRelative("compareType").enumValueIndex;

			switch(Tipo)
			{
				case ListaElementos.FormasComparacion.floatEnum:
					EditorGUI.PropertyField(new Rect(rect.x + (rect.width * 2 / 3), rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("floatField"), GUIContent.none );
				break;

				case ListaElementos.FormasComparacion.inputFieldEnum:
					EditorGUI.PropertyField(new Rect(rect.x + (rect.width * 2 / 3), rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("inputField"), GUIContent.none );
				break;

				case ListaElementos.FormasComparacion.intEnum:
					EditorGUI.PropertyField(new Rect(rect.x + (rect.width * 2 / 3), rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("integerField"), GUIContent.none );
				break;

				case ListaElementos.FormasComparacion.stringEnum:
					EditorGUI.PropertyField(new Rect(rect.x + (rect.width * 2 / 3), rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("stringField"), GUIContent.none );
				break;

				case ListaElementos.FormasComparacion.textEnum:
					EditorGUI.PropertyField(new Rect(rect.x + (rect.width * 2 / 3), rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("textField"), GUIContent.none );
				break;

				case ListaElementos.FormasComparacion.toggleEnum:
					EditorGUI.PropertyField(new Rect(rect.x + (rect.width * 2 / 3), rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("toggleField"), GUIContent.none );
				break;

				default:
				break;
			}

			

		};


        listaHeaders = new ReorderableList(serializedObject, serializedObject.FindProperty("headersClass"), true, true, true, true);

        listaHeaders.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
        {
            var elemento = listaHeaders.serializedProperty.GetArrayElementAtIndex(index);


            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("keyName"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + (rect.width / 3), rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("compareType"), GUIContent.none);

            ListaElementos.FormasComparacion Tipo = (ListaElementos.FormasComparacion)elemento.FindPropertyRelative("compareType").enumValueIndex;

            switch (Tipo)
            {
                case ListaElementos.FormasComparacion.floatEnum:
                    EditorGUI.PropertyField(new Rect(rect.x + (rect.width * 2 / 3), rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("floatField"), GUIContent.none);
                    break;

                case ListaElementos.FormasComparacion.inputFieldEnum:
                    EditorGUI.PropertyField(new Rect(rect.x + (rect.width * 2 / 3), rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("inputField"), GUIContent.none);
                    break;

                case ListaElementos.FormasComparacion.intEnum:
                    EditorGUI.PropertyField(new Rect(rect.x + (rect.width * 2 / 3), rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("integerField"), GUIContent.none);
                    break;

                case ListaElementos.FormasComparacion.stringEnum:
                    EditorGUI.PropertyField(new Rect(rect.x + (rect.width * 2 / 3), rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("stringField"), GUIContent.none);
                    break;

                case ListaElementos.FormasComparacion.textEnum:
                    EditorGUI.PropertyField(new Rect(rect.x + (rect.width * 2 / 3), rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("textField"), GUIContent.none);
                    break;

                case ListaElementos.FormasComparacion.toggleEnum:
                    EditorGUI.PropertyField(new Rect(rect.x + (rect.width * 2 / 3), rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), elemento.FindPropertyRelative("toggleField"), GUIContent.none);
                    break;

                default:
                    break;
            }
        };

	}


	/// <summary>
	/// Raises the inspector GU event.
	/// </summary>
	public override void OnInspectorGUI()
	{
		// Actualiza los valores de la clase ConsultasAPI
		serializedObject.Update ();

		// Pone en el inspector la cadena de DireccionAPI
		EditorGUILayout.PropertyField ( RutaAPI );

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Headers");
        listaHeaders.DoLayoutList();

        // Dibuja la lista de condiciones
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("POST Values");
        EditorGUILayout.LabelField("Key -> Values");
        ListaCondiciones.DoLayoutList ();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Eventos al finalizar consulta de API");
        EditorGUILayout.PropertyField ( ListaCasosExito, true );
        EditorGUILayout.PropertyField(EventoBien);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField ( ListaCasosVacio, true );
        EditorGUILayout.PropertyField(EventoNul);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField ( ListaCasosError, true );
        EditorGUILayout.PropertyField(EventoMal);

		// Aplica las propiedades modificadas
		serializedObject.ApplyModifiedProperties ();
	}


}
