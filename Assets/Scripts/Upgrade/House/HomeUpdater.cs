
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class HomeUpdater : MonoBehaviour
{

    private House _currentHouse;
    private const string _houselayer = "House";
    private const string _UIlayer = "UI";
    private int _houseMask;

    private const string _mouseButton = "Fire1";


    public bool isMove;

    [Header("Canvas")]
    [Space]
    [SerializeField] private GameObject _houseInfo;
    [SerializeField] private GameObject _upgradeMenu;

    [Header("Button")]
    [Space]
    [SerializeField] private GameObject _upgradeButton;


    [Header("House Info")]
    [Space]
    [SerializeField] private Text _propertyHouse;
    [SerializeField] private Text _rob;
    [SerializeField] private Text _securityProtected;
    
    
    [Header("Icons Update")]
    [Space]
    [SerializeField] private GameObject _iconZabor;
    [SerializeField] private GameObject _iconSignalization;
    [SerializeField] private GameObject _iconCamera;


    [Header("Upgrade Button")]
    [Space]
    [SerializeField] private GameObject _updateButton_Zabor;
    [SerializeField] private GameObject _updateButton_Signalization;
    [SerializeField] private GameObject _updateButton_Camera;

    private void Awake()
    {
        _houseMask = 1 << LayerMask.NameToLayer(_houselayer);
    }  
    
    private void Start()
    {
        StartCoroutine(FakeSignalization());

    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Input.GetButtonDown(_mouseButton))
        {

            if (EventSystem.current.IsPointerOverGameObject(0)) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (Input.touchCount > 0)  if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return; 

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _houseMask))
            {
                isMove = true;
            }
        }


        if (Input.GetButtonUp(_mouseButton) && isMove)
        {
            if (TargetsManager.Instance.isBuildMode) return;
            if (EventSystem.current.IsPointerOverGameObject(0)) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (Input.touchCount > 0) if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _houseMask))
            {
                CloseUI();
                OpenUI(hit.collider.gameObject.GetComponent<House>());

            }

        }

        if (Input.GetButtonDown(_mouseButton))
        {
            if (EventSystem.current.IsPointerOverGameObject(0)) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (Input.touchCount > 0) if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, _houseMask))
            {
                CloseUI();
            }

        }
    }

    IEnumerator FakeSignalization()
    {
        while (true)
        {
            yield return new WaitForSeconds(30);
            House house = TargetsManager.Instance.houses[Random.Range(0, TargetsManager.Instance.houses.Count - 1)];
            if (house.upg_zabor_or_signalization && !house.going_to_rob && !house.rob && !house.securityProtected && !house._isAHouse)
            {
                MainPlayer.Instance.ShowMessage("Сработала ложная сигнализация");
                house.rob = true;
                house.StartRobbery();

                int time = 30;
                while (time > 0)
                {
                    yield return new WaitForSeconds(1);
                    time--;
                    house.marker.GetComponent<UITimer>().SetNewTime("" + time, true);
                }
               
                if (house.rob)
                {
                    MainPlayer.Instance.ShowMessage("Вы не отреагировали на вызов");
                    house.FakeSignalizationFailure();

                }

            }

        }
    }



    private void OpenUI(House house)
    {

        _currentHouse?.OutlineOff();
        _currentHouse = house;
        _currentHouse.OutlineOn();
        _houseInfo.SetActive(true);
        SetDataToUi();
        Time.timeScale = 0;
    }

    private void CloseUI()
    {
        _currentHouse?.OutlineOff();
        _currentHouse = null;
        _houseInfo.SetActive(false);
        _upgradeMenu.SetActive(false);
        _upgradeButton.SetActive(true);
        isMove = false;

        // отключаем все икноки полученных улучшений
        _iconZabor.SetActive(false);
        _iconSignalization.SetActive(false);
        _iconCamera.SetActive(false);

        
        // Отключаем обе что бы при следующем включении включилась одна из
        _updateButton_Zabor.SetActive(false);
        _updateButton_Signalization.SetActive(false);

        // Возвращем прозрачность на 100%
        _updateButton_Zabor.GetComponent<Image>().CrossFadeAlpha(1,0,false);
        _updateButton_Signalization.GetComponent<Image>().CrossFadeAlpha(1,0,false);
        _updateButton_Camera.GetComponent<Image>().CrossFadeAlpha(1,0,false);



        _updateButton_Zabor.GetComponent<ButtonSize>().enabled = true;
        _updateButton_Zabor.GetComponent<EventTrigger>().enabled = true;
        _updateButton_Signalization.GetComponent<ButtonSize>().enabled = true;
        _updateButton_Signalization.GetComponent<EventTrigger>().enabled = true;
        _updateButton_Camera.GetComponent<ButtonSize>().enabled = true;
        _updateButton_Camera.GetComponent<EventTrigger>().enabled = true;

        Time.timeScale = 1;

    }

    private void SetDataToUi()    
    {

        //#### Home Info
        _propertyHouse.text = ""+_currentHouse.GetCurrentProerty();
        _securityProtected.text = ""+(_currentHouse.securityProtected? "Под защитой" : "");
        _rob.text = "" + (_currentHouse.going_to_rob ? (_currentHouse.rob ? "Грабят" : "Едут грабить"):" В безопасности");


        //#### Button
        if (_currentHouse.going_to_rob || _currentHouse.rob)
        {
            _upgradeButton.SetActive(false);
        }



        //#### Show Update icon and inactive button
        if (_currentHouse._isAHouse)
        {
            _updateButton_Zabor.SetActive(true);
            if (_currentHouse.upg_zabor_or_signalization)
            {
                _iconZabor.SetActive(true);
                _updateButton_Zabor.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
                _updateButton_Zabor.GetComponent<ButtonSize>().enabled = false;
                _updateButton_Zabor.GetComponent<EventTrigger>().enabled = false;
            }
        }
        else
        {
            _updateButton_Signalization.SetActive(true);
            if (_currentHouse.upg_zabor_or_signalization)
            {
                _iconSignalization.SetActive(true);
                _updateButton_Signalization.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
                _updateButton_Signalization.GetComponent<ButtonSize>().enabled = false;
                _updateButton_Signalization.GetComponent<EventTrigger>().enabled = false;
            }

        }


        if (_currentHouse.upg_camera)
        {
            _iconCamera.SetActive(true);
            _updateButton_Camera.GetComponent<Image>().CrossFadeAlpha(0.5f,0,false);
            _updateButton_Camera.GetComponent<ButtonSize>().enabled = false;
            _updateButton_Camera.GetComponent<EventTrigger>().enabled = false;
        }      
        



    }

    public void OpenUpgradeMenu() {
        _upgradeMenu.SetActive(true);
        _houseInfo.SetActive(false);    
    }

    
    
    public void UpgradeHouse(int type) {
        bool r = _currentHouse.UpgradeHouse(type);
        if (r) {
            // #### inactive button 
            switch (type)
            {
                case 1:
                    _updateButton_Zabor.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
                    _updateButton_Zabor.GetComponent<ButtonSize>().enabled = false;
                    _updateButton_Zabor.GetComponent<EventTrigger>().enabled = false;

                    _updateButton_Signalization.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
                    _updateButton_Signalization.GetComponent<ButtonSize>().enabled = false;
                    _updateButton_Signalization.GetComponent<EventTrigger>().enabled = false;

                    break;
                case 2:
                    _updateButton_Camera.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
                    _updateButton_Camera.GetComponent<ButtonSize>().enabled = false;
                    _updateButton_Camera.GetComponent<EventTrigger>().enabled = false;
                    break;
                default:
                    break;
            }
        }
    }
}
