using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FactoriesSecurity : MonoBehaviour
{
    [SerializeField] GameObject _car;
    [SerializeField] Transform _spawnPoint;

    public int carsCount;
    public int maxCount;

    public int distToSpawn = 5;

    public List<GameObject> _spawnedCars;
    public Transform _lastSpawnObject;
    public Outline _outline;

    public GameObject isUpdate;


    [Header("Updates")] 
    public bool upg_moreCar;
    
    public bool upg_teach; // Скорость ++
    public bool upg_powerUp; // Время --
    public bool upg_tablet; // Скорость ++
    public bool upg_radio; // Включение определения маршрута
    public bool upg_fleshers; // Скорость ++


    [Header("Button")]
    [Space]
    [SerializeField] private GameObject _expertButton;

    public float time = 0;
    public float timer = 10;


    IEnumerator CheckUpdatePossible()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if ((upg_moreCar && upg_teach && upg_powerUp && upg_tablet && upg_radio && upg_fleshers) || MainPlayer.Instance.Money < 1000)
            {
                isUpdate.SetActive(false);
            }
            else
            {
                isUpdate.SetActive(true);
            }
        }
    }


    [ContextMenu("Expert")]
    public void Expert()
    {
        MainPlayer.Instance.ShowMessage("Все грабители исчезли с карты");

        for (int i = 0; i < TargetsManager.Instance.robbersInLevel.Count; i++)
        {
            TargetsManager.Instance.robbersInLevel[i].GetComponent<RoberryPathFinder>().movePositionHouse.ExpertDestroyAll(); 
        }

        
        time = Time.unscaledTime;


        _expertButton.GetComponent<ButtonSize>().enabled = false;
        _expertButton.GetComponent<EventTrigger>().enabled = false;
        _expertButton.GetComponent<Button>().interactable = false;
        // }

    }



    public void ResetButtonExpert()
    {
        if (!_expertButton.GetComponent<Button>().interactable)
        {
           // Debug.Log("Перезапустили кнопку Эксперт");

            _expertButton.GetComponent<ButtonSize>().enabled = true;
            _expertButton.GetComponent<EventTrigger>().enabled = true;
            _expertButton.GetComponent<Button>().interactable = true;
        }

    }

    private void Start()
    {
        _expertButton.GetComponent<Image>().CrossFadeAlpha(0.5f, 0, false);
        _expertButton.GetComponent<ButtonSize>().enabled = false;
        _expertButton.GetComponent<EventTrigger>().enabled = false;
        _expertButton.GetComponent<Button>().interactable = false;

        maxCount = carsCount;

        if (upg_moreCar)
        {
            maxCount++;
        }

        SpawnSecurityCar();
        StartCoroutine(CarSpawner());

        _outline = GetComponent<Outline>();

        StartCoroutine(CheckUpdatePossible());
    }


    private bool Upgrader(int price, string name)
    {
        if (MainPlayer.Instance.Money - price >= 0)
        {
            MainPlayer.Instance.Money = -price;
            AchivemntController.Instance.AchFirstUpdate();
            SaveLoadController.SaveOut();
            // Debug.Log("Приобрели "+name);
            return true;
        }
        else
        {
            MainPlayer.Instance.ShowMessage("Не хватает денег");
            // Debug.Log("Не хватает денег");ShowMessage
            return false;
        }
    }

    public void OutlineOn()
    {
        _outline.enabled = true;
    }

    public void OutlineOff()
    {
        _outline.enabled = false;
    }

    private bool UpgradeMoreCar() {
        int price = 1000;
        string name = "Больше машин" ;

        bool result = Upgrader(price, name);
        if (result) upg_moreCar = true;
        carsCount++;
        maxCount++;


        return result;
    }    
    
    private bool UpgradeTeach() {
        int price = 1000;
        string name = "Обучение сотрудников" ;

        bool result = Upgrader(price, name);
        if (result) upg_teach = true;
        return result;
    }   
    
    private bool UpgradePower() {
        int price = 1000;
        string name = "Сила сотрудников" ;

        bool result = Upgrader(price, name);
        if (result) upg_powerUp = true;
        return result;
    }  
    
    private bool UpgradeTablet() {
        int price = 1000;
        string name = "Планшет" ;

        bool result = Upgrader(price, name);
        if (result) upg_tablet = true;
        return result;
    }    
    
    private bool UpgradeRadio() {
        int price = 1000;
        string name = "Рация" ;

        bool result = Upgrader(price, name);
        if (result) upg_radio = true;
        return result;
    }    
    
    private bool UpgradeFleshers() {
        int price = 1000;
        string name = "Мигалки" ;

        bool result = Upgrader(price, name);
        if (result) upg_fleshers = true;
        return result;
    }


    public bool UpgradeHouse(int type)
    {
        switch (type)
        {
            case 1:
                return UpgradeMoreCar();
                break;
            case 2:
                return UpgradeTeach();
                break;
            case 3:
                return UpgradePower();
                break;
            case 4:
                return UpgradeTablet();
                break;
            case 5:
                return UpgradeRadio();
                break;
            case 6:
                return UpgradeFleshers();
                break;
            default:
               // Debug.LogError("Не понимаю что нужно улучшить");
                return false;
                break;
        }
    }





    public void HelpConsultation()
    {
        //Debug.Log("Убирает всех грабителей с карты");
        //Убирает всех грабителей с карты.
    }

    IEnumerator CarSpawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            SpawnSecurityCar();
        }
    }

    [ContextMenu("SpawnSecurityCar")]
    public void SpawnSecurityCar() {
        if (carsCount > 0 && GetDistance())
        {
            var car = Instantiate(_car, _spawnPoint.position, _spawnPoint.rotation);

            _lastSpawnObject = car.GetComponent<Transform>();
            UpgraderCar(car.GetComponent<SecurityController>());
            if (upg_fleshers)
            {
                car.GetComponent<SecurityController>().SerenOn();
            }
            _spawnedCars.Add(car);
            carsCount--;
        }
       
    }

    private void UpgraderCar(SecurityController car)
    {
        
    /*public bool upg_teach; // Скорость ++
    public bool upg_powerUp; // Время --
    public bool upg_tablet; // Скорость ++
    public bool upg_radio; // Включение определения маршрута
    public bool upg_fleshers; // Скорость ++*/
        
        
        if (upg_teach) car.SpeedUp();
        if (upg_powerUp) car.ArestSpeed();
        if (upg_tablet) car.SpeedUp();
        if (upg_radio) car.BestRoad();
        if (upg_fleshers) car.SpeedUp();
        
        if (upg_teach && upg_powerUp && upg_tablet &&upg_radio && upg_fleshers) car.AutoPilot();
    }

    private bool GetDistance() {
        if (_lastSpawnObject == null) return true;

        if (Vector3.Distance(transform.position, _lastSpawnObject.position) > distToSpawn) {
            return true;
        }
        return false;
    }
/*
    public void CarEscape(GameObject spawnedCar) {
        carsCount++;
        _spawnedCars.Remove(spawnedCar);
    }*/

}
