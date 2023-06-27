using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.IO;
using System.Text;
using Unity.Netcode.Transports.UTP;

public class PauseMenu : NetworkBehaviour{
    [SerializeField] private GameObject NetworkManagerObject; 
    [SerializeField] private GameObject MenuObject; 
    [SerializeField] private GameObject ColorChosingObject; 
    [SerializeField] private Transform colorSwitchingPlace;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Health playerHealth;
    [SerializeField] private GameObject IPText;
    [SerializeField] private KeyBindData keyBind;
    [SerializeField] private Identift iden;

    [SerializeField] private Button pauseMenuButton;

    private string myAddressLocal;
    private string myAddressGlobal;
    private bool isActive = false;

    private void Start() {
        Debug.Log("pauseStart");
        IPInit();
    }

    private void Update() {
        if(!iden.isTheOwner()) Destroy(gameObject);
        Debug.Log(iden.isTheOwner());
    }
    private void Awake() {
        pauseMenuButton.onClick.AddListener(() => {
            Debug.Log("pause");
            NetworkManagerObject = GameObject.Find("NetworkManager").gameObject;
            isActive = !isActive;
            if(isActive) keyBind.inType = 3;
            else keyBind.inType = 0;
            if(NetworkManagerObject.GetComponent<UnityTransport>().ConnectionData.Address == "0.0.0.0") NetworkManagerObject.GetComponent<UnityTransport>().ConnectionData.Address = myAddressLocal;
            IPText.GetComponent<TMPro.TextMeshProUGUI>().text = "IP: " + NetworkManagerObject.GetComponent<UnityTransport>().ConnectionData.Address;
            MenuObject.SetActive(isActive);
        });
    }
    public void switchColorButton() {
        continueButton();
        // menuOpenButton.SetActive(false);
        playerTransform.position = colorSwitchingPlace.position;
        playerHealth.dead.Value = true;
        ColorChosingObject.SetActive(true);
    }
    public void continueButton() {
        isActive=false;
        keyBind.inType = 0;
        MenuObject.SetActive(false);
    }
    public void exitButton() {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(0);
    }

    private void IPInit() {
        //Get the local IP
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach(IPAddress ip in hostEntry.AddressList) {
            if(ip.AddressFamily == AddressFamily.InterNetwork) {
                myAddressLocal = ip.ToString();
                break;
            } //if
        } //foreach
        //Get the global IP
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.ipify.org");
        request.Method = "GET";
        request.Timeout = 1000; //time in ms
        try {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if(response.StatusCode == HttpStatusCode.OK) {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                myAddressGlobal = reader.ReadToEnd();
            } //if
            else {
                Debug.LogError("Timed out? " + response.StatusDescription);
                myAddressGlobal = "127.0.0.1";
            } //else
        } //try
        catch(WebException ex) {
            Debug.Log("Likely no internet connection: " + ex.Message);
            myAddressGlobal = "127.0.0.1";
        } //catch
        //myAddressGlobal=new System.Net.WebClient().DownloadString("https://api.ipify.org"); //single-line solution for the global IP, but long time-out when there is no internet connection, so I prefer to do the method above where I can set a short time-out time
    } //Start
}
