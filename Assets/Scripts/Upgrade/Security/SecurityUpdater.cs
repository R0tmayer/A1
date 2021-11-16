using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SecurityUpdater : MonoBehaviour
{
    private FactoriesSecurity _currentSecurity;
    private const string _securitylayer = "SecurityHouse";
    private int _securityMask;
    private const string _mouseButton = "Fire1";

    public bool isMove;
    private bool checkerExpertReload = true;

    public Text timer;

    [Header("Canvas")]
    [Space]
    [SerializeField] private GameObject _houseInfo;
    [SerializeField] private GameObject _upgradeMenu;

    [Header("House Info")]
    [Space]
    [SerializeField] private Text _curentCars;
    [SerializeField] private Text _allCars;    
    
    [Header("House Info")]
    [Space]
    [SerializeField] private Text _house;
    [SerializeField] private Text _houseAll;


    [Header("Icons Update")]
    [Space]
    [SerializeField] private GameObject _iconMoreCar;
    [SerializeField] private GameObject _iconTeach;
    [SerializeField] private GameObject _iconPowerUp;
    [SerializeField] private GameObject _iconTablet;
    [SerializeField] private GameObject _iconRadio;
    [SerializeField] private GameObject _iconFleshers;


    [Header("Upgrade Button")]
    [Space]
    [SerializeField] private GameObject _updateButton_MoreCar;
    [SerializeField] private GameObject _updateButton_Teach;
    [SerializeField] private GameObject _updateButton_PowerUp;
    [SerializeField] private GameObject _updateButton_Tablet;
    [SerializeField] private GameObject _updateButton_Radio;
    [SerializeField] private GameObject _updateButton_Flesher;


    private void Awake()
    {
        _securityMask = 1 << LayerMask.NameToLayer(_securitylayer);
    }


    private void SetCountHouses() {
        TargetsManager.Instance.countUpdate = 0;
        foreach (var item in TargetsManager.Instance.houses)
        {
            if (item.upg_camera || item.upg_zabor_or_signalization)
            {
                TargetsManager.Instance.countUpdate++;
            }      
        }

        _house.text = TargetsManager.Instance.countUpdate.ToString();
        _houseAll.text = TargetsManager.Instance.houses.Count.ToString();
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (_currentSecurity != null)
        {
            int i = (int)(_currentSecurity.time + _currentSecurity.timer - Time.unscaledTime);
            if (i > 0)
            {
                checkerExpertReload = true;
                timer.text = "" + i;
                
            }
            else {

                if (checkerExpertReload == true)
                {
                    MainPlayer.Instance.ShowMessage("Эксперт доступен");
                    _currentSecurity.ResetButtonExpert();
                    timer.text = "";
                    checkerExpertReload = false;
                }
        
            } 
        }



        if (Input.GetButtonDown(_mouseButton))
        {
        

            if (EventSystem.current.IsPointerOverGameObject(0)) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (Input.touchCount > 0) if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;


            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _securityMask))
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


            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _securityMask))
            {
                CloseUI();
                OpenUI(hit.collider.gameObject.GetComponent<FactoriesSecurity>());

            }

        }

        if (Input.GetButtonDown(_mouseButton))
        {
            if (EventSystem.current.IsPointerOverGameObject(0)) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (Input.touchCount > 0) if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, _securityMask))
            {
                CloseUI();
            }
        }
    }


    private void OpenUI(FactoriesSecurity house)
    {
        SetCountHouses();
        _currentSecurity?.OutlineOff();
        _currentSecurity = house;
        _currentSecurity.OutlineOn();
        _houseInfo.SetActive(true);
        SetDataToUi();

        Time.timeScale = 0;
    }

    private void CloseUI()
    { 
        _currentSecurity?.OutlineOff();
/*        _currentSecurity = null;*/
        _houseInfo.SetActive(false);
        _upgradeMenu.SetActive(false);
        isMove = false;


        // отключаем все икноки полученных улучшений
        _iconMoreCar.SetActive(false);
        _iconTeach.SetActive(false);
        _iconPowerUp.SetActive(false);
        _iconTablet.SetActive(false);
        _iconRadio.SetActive(false);
        _iconFleshers.SetActive(false);


        // Возвращем прозрачность на 100%
        _updateButton_MoreCar.GetComponent<Image>().CrossFadeAlpha(1, 0, false);
        _updateButton_Teach.GetComponent<Image>().CrossFadeAlpha(1, 0, false);
        _updateButton_PowerUp.GetComponent<Image>().CrossFadeAlpha(1, 0, false);
        _updateButton_Tablet.GetComponent<Image>().CrossFadeAlpha(1, 0, false);
        _updateButton_Radio.GetComponent<Image>().CrossFadeAlpha(1, 0, false);
        _updateButton_Flesher.GetComponent<Image>().CrossFadeAlpha(1, 0, false);



        _updateButton_MoreCar.GetComponent<ButtonSize>().enabled = true;
        _updateButton_MoreCar.GetComponent<EventTrigger>().enabled = true;
        _updateButton_Teach.GetComponent<ButtonSize>().enabled = true;
        _updateButton_Teach.GetComponent<EventTrigger>().enabled = true;
        _updateButton_PowerUp.GetComponent<ButtonSize>().enabled = true;
        _updateButton_PowerUp.GetComponent<EventTrigger>().enabled = true;
        _updateButton_Tablet.GetComponent<ButtonSize>().enabled = true;
        _updateButton_Tablet.GetComponent<EventTrigger>().enabled = true;
        _updateButton_Radio.GetComponent<ButtonSize>().enabled = true;
        _updateButton_Radio.GetComponent<EventTrigger>().enabled = true;
        _updateButton_Flesher.GetComponent<ButtonSize>().enabled = true;
        _updateButton_Flesher.GetComponent<EventTrigger>().enabled = true;

        Time.timeScale = 1;
    }


    private void SetDataToUi()
    {

        //#### Home Info
        _curentCars.text = "" + _currentSecurity.carsCount;
        _allCars.text = "" + _currentSecurity.maxCount;




        //#### Show Update icon and inactive button
        if (_currentSecurity.upg_moreCar)
        {
            _iconMoreCar.SetActive(true);
            _updateButton_MoreCar.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
            _updateButton_MoreCar.GetComponent<ButtonSize>().enabled = false;
            _updateButton_MoreCar.GetComponent<EventTrigger>().enabled = false;
        }
 
        if (_currentSecurity.upg_teach)
        {
            _iconTeach.SetActive(true);
            _updateButton_Teach.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
            _updateButton_Teach.GetComponent<ButtonSize>().enabled = false;
            _updateButton_Teach.GetComponent<EventTrigger>().enabled = false;
        }

        if (_currentSecurity.upg_powerUp)
        {
            _iconPowerUp.SetActive(true);
            _updateButton_PowerUp.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
            _updateButton_PowerUp.GetComponent<ButtonSize>().enabled = false;
            _updateButton_PowerUp.GetComponent<EventTrigger>().enabled = false;
        }

        if (_currentSecurity.upg_tablet)
        {
            _iconTablet.SetActive(true);
            _updateButton_Tablet.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
            _updateButton_Tablet.GetComponent<ButtonSize>().enabled = false;
            _updateButton_Tablet.GetComponent<EventTrigger>().enabled = false;
        }

        if (_currentSecurity.upg_radio)
        {
            _iconRadio.SetActive(true);
            _updateButton_Radio.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
            _updateButton_Radio.GetComponent<ButtonSize>().enabled = false;
            _updateButton_Radio.GetComponent<EventTrigger>().enabled = false;
        }

        if (_currentSecurity.upg_fleshers)
        {
            _iconFleshers.SetActive(true);
            _updateButton_Flesher.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
            _updateButton_Flesher.GetComponent<ButtonSize>().enabled = false;
            _updateButton_Flesher.GetComponent<EventTrigger>().enabled = false;
        }
 

    }

    public void OpenUpgradeMenu()
    {
        _upgradeMenu.SetActive(true);
        _houseInfo.SetActive(false);
    }



    public void UpgradeHouse(int type)
    {
        bool r = _currentSecurity.UpgradeHouse(type);
        if (r)
        {
            // #### inactive button 
            switch (type)
            {
                case 1:
                    _updateButton_MoreCar.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
                    _updateButton_MoreCar.GetComponent<ButtonSize>().enabled = false;
                    _updateButton_MoreCar.GetComponent<EventTrigger>().enabled = false;
                    break;
                case 2:
                    _updateButton_Teach.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
                    _updateButton_Teach.GetComponent<ButtonSize>().enabled = false;
                    _updateButton_Teach.GetComponent<EventTrigger>().enabled = false;
                    break;
                case 3:
                    _updateButton_PowerUp.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
                    _updateButton_PowerUp.GetComponent<ButtonSize>().enabled = false;
                    _updateButton_PowerUp.GetComponent<EventTrigger>().enabled = false;
                    break;
                case 4:
                    _updateButton_Tablet.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
                    _updateButton_Tablet.GetComponent<ButtonSize>().enabled = false;
                    _updateButton_Tablet.GetComponent<EventTrigger>().enabled = false;
                    break;
                case 5:
                    _updateButton_Radio.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
                    _updateButton_Radio.GetComponent<ButtonSize>().enabled = false;
                    _updateButton_Radio.GetComponent<EventTrigger>().enabled = false;
                    break;
                case 6:
                    _updateButton_Flesher.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
                    _updateButton_Flesher.GetComponent<ButtonSize>().enabled = false;
                    _updateButton_Flesher.GetComponent<EventTrigger>().enabled = false;
                    break;
                default:
                    break;
            }
        }
    }

}
