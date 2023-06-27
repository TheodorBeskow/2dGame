using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button clientbtn;
    [SerializeField] private Button hostbtn;
    [SerializeField] private GameObject inputField;
    [SerializeField] private UnityTransport unityTransport;

    private void Awake() {
        clientbtn.onClick.AddListener(() => {
            string text = inputField.GetComponent<TMP_InputField>().text;
            unityTransport.ConnectionData.Address = text;
            NetworkManager.Singleton.StartClient();
            Chosen();
        });
        hostbtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            Debug.Log(unityTransport.ConnectionData.Address);
            Chosen();
        });
    }

    private void Chosen() {
        gameObject.SetActive(false);
    }
}
