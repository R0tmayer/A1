using PlayerCar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;


public class House : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] public bool _isAHouse; // Дом или бизнес здание
    [SerializeField] public GameObject _targetPoint;
    
    [Header("Canvas")][Space]

    public GameObject marker;

    [Header("Audio")]
    [Space]
    public Pack pack;
    public AudioSource audioSource;
    public AudioClip succesAudio;
    public AudioClip failAudio;
    [Header("Particle")]
    [Space]
    public GameObject signalParticle;
    public GameObject signalWarning;
    public GameObject successParticle;
    public GameObject failParticle;
    [Header("Particle")]
    [Space]
    public GameObject isUpdate;

    private bool _checkUpdate;


    [Header("Automatic Find")]
    public Transform transformForPathFinder; //Нужная для pathFinderRobbierУ
    public NavMeshAgent houseRob; //Нужная для pathFinderRobbierУ
    public SecurityController security; //Нужная для pathFinderRobbierУ
    public GameObject zabor;
    private TargetPont _vfx;
    private Outline _outline;

    private FirebaseManager firebase;


    [Header("States")]
    [SerializeField] public bool going_to_rob; //Едут на ограбление
    [SerializeField] public bool rob; // Начали грабить
    [SerializeField] public bool securityProtected; // Под защитой
    private float _property = 100; // Текущее значение имущества
    [SerializeField] private float _maxProperty = 100; // Максимально количество имузества (для сброса по умолчанию, его так же можно увеличивать)

    public event Action OnCompleteRobbir; // Если ограбление заканчивается

    [Header("Updates")]
    public bool upg_zabor_or_signalization;
    public bool upg_camera;


    public void SetSecurityProtected() {
        securityProtected = true;
        marker.GetComponent<UITimer>().child.HideImage(false);
    }

    public void Awake()
    {
        
        _targetPoint = pack._targetPoint;
        marker = pack.marker;
        signalParticle = pack.signalParticle;
        signalWarning = pack.signalWarning;
        successParticle = pack.successParticle;
        failParticle = pack.failParticle;

        _vfx = _targetPoint.GetComponent<TargetPont>();
        transformForPathFinder = _targetPoint.GetComponent<Transform>();

    }
    private void Start()
    {
        firebase = FindObjectOfType<FirebaseManager>();
        marker.GetComponent<Canvas>().worldCamera = Camera.main;
        _vfx.SetParentHouse(this);
        _outline = GetComponent<Outline>();
        //StartCoroutine(CheckUpdatePossible());
    }

/*    IEnumerator CheckUpdatePossible() {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (rob || going_to_rob || securityProtected || (upg_zabor_or_signalization && upg_camera) || MainPlayer.Instance.Money < 100 )
            {
                isUpdate.SetActive(false);
            }
            else
            {
                isUpdate.SetActive(true);
            }
        }
    }*/

    private void Update() {
        if (!_checkUpdate)
        {
            _checkUpdate = true;
            if (upg_zabor_or_signalization)
            {
                CheckUpgradeTime();
            }
        }
    }

    // Сработает при улучшении дома
    public float MaxPropert
    {
        set
        {
            _maxProperty += value;
            _property = _maxProperty;
        }
    }

    public float GetCurrentProerty()
    {
        return _maxProperty;
    }




    //Коэфициент для отнимания имущества
    private float k = 1;
    public float Property
    {
        get => _property;
        set
        {
            if ((_property - value) <= 0)
            {
                OnCompleteRobbir?.Invoke();
                EndRobbery();
                ProtectionFailure();
                return;
            }
            _property -= value * k; // Каждый раз при обращении будет отнимать 1 или (0,1-0,9) в зависимости от коэфициента 
        }
    }

    public bool UpgradeHouse(int type) {
        switch (type)
        {
            case 1:
                return UpgradeTime();
                break;
            case 2:
                return UpgradeCamera();
                break;
            default:
                Debug.LogError("Не понимаю что нужно улучшить");
                return false;
                break;
        }
    }


    // ### Updates
    public void AchChecker() {
        if (upg_zabor_or_signalization && upg_camera)
        {

            if ((TargetsManager.Instance.countUpdate == TargetsManager.Instance.houses.Count))
            {
                foreach (var item in TargetsManager.Instance.houses)
                {
                    if (!item.upg_zabor_or_signalization || !item._checkUpdate)
                    {
                        break;
                    }
                    else {
                        AchivemntController.Instance.AchUpdate();
                        return;
                    }
                }

                
            }

            AchivemntController.Instance.AchAddUpdateHome();
        }
    }

    private bool UpgradeTime() {
        int price = 50;

        if (MainPlayer.Instance.Money - price >= 0)
        {
            MainPlayer.Instance.Money = -price;

            upg_zabor_or_signalization = true;
            MaxPropert = 100;
            AchivemntController.Instance.AchFirstUpdate();

            AchChecker();
            SaveLoadController.SaveOut();
            if (_isAHouse)
            {
                zabor.SetActive(true);
                return true;
            }
            else
            {
                signalParticle.SetActive(true);
                return true;
            }

        }
        else {

            MainPlayer.Instance.ShowMessage("Не хватает денег");
            return false;
        }
    }


    private void CheckUpgradeTime() {
        MaxPropert = 100;
        if (_isAHouse)
        {
            zabor.SetActive(true);
        }
        else {
            signalParticle.SetActive(true);
        }
    }



    private bool UpgradeCamera() {
        int price = 100;

        if (MainPlayer.Instance.Money - price >= 0)
        {
            MainPlayer.Instance.Money = -price;
            upg_camera = true;
            AchivemntController.Instance.AchFirstUpdate();
            AchChecker();
            SaveLoadController.SaveOut();
            return true;
        }
        else
        {
            MainPlayer.Instance.ShowMessage("Не хватает денег");
            return false;
        }
    }


    //  Updates ###

    public void IsBeingProtected()
    {
        OnCompleteRobbir?.Invoke();
        ProtectionSucces();
        EndRobbery();
        return;
    }




    public void OutlineOn() {
        _outline.enabled = true;
    }

    public void OutlineOff() {
        _outline.enabled = false;
    }



    public void StartRobbery() {
        _vfx.VfxOn();
        signalWarning.SetActive(true);
        marker.SetActive(true);
        going_to_rob = true;

    }
    public void EndRobbery()
    {
        _vfx.VfxOff();
        signalWarning.SetActive(false);
        marker.SetActive(false);
        going_to_rob = false;
        rob = false;
        _property = _maxProperty;
    }


    public void Rob() {
        rob = true;
    }

    private void AddScoreToBase()
    {
        if (!GuestHolder.state)
        {
            if (MainPlayer.Instance.Raiting > ExperienceHolder.value)
            {
                ExperienceHolder.value = MainPlayer.Instance.Raiting;
                firebase.UpdateFirebaseUserData();
            }
        }
        else {
            Debug.Log("Рекорд побит, но вы не авторизованы");
        }

    }

    [ContextMenu("ProtectionSucces")]
    public void ProtectionSucces() {
        audioSource.PlayOneShot(succesAudio);
        ViewHousStatusParticle(successParticle);
        EndRobbery();
        //Тут считаем очки, прибавляем или отнимаем и сколько
        //Debug.Log("Ограбление предотвращено");
        /* MainPlayer.Instance.HouseState = "Ограбление предотвращено =)";*/
        MainPlayer.Instance.Raiting = 50;
        AddScoreToBase();
        MainPlayer.Instance.Money = 50;
        TargetsManager.Instance.countUpdate++;
        if (TargetsManager.Instance.countUpdate >= 20)
        {
            AchivemntController.Instance.AchProtectedAbsolute();
        }
        SaveLoadController.SaveOut();
        //TargetsManager.Instance.factoriesSecurity.carsCount++;
    }

    [ContextMenu("ProtectionFailure")]
    public void ProtectionFailure() {
        audioSource.PlayOneShot(failAudio);
        ViewHousStatusParticle(failParticle);
        EndRobbery();
  /*      MainPlayer.Instance.HouseState = "ОГРАБИЛИ =(";*/
        MainPlayer.Instance.Raiting = upg_camera ? -25 : -50;
        MainPlayer.Instance.Money = upg_camera ? -25 : -50;
        TargetsManager.Instance.countUpdate = 0;
        SaveLoadController.SaveOut();
        //Тут считаем очки, прибавляем или отнимаем и сколько
    }

    
    public void FakeSignalizationFailure()
    {
        audioSource.PlayOneShot(failAudio);
        ViewHousStatusParticle(failParticle);
        EndRobbery();
        MainPlayer.Instance.Raiting = -50;
        TargetsManager.Instance.countUpdate = 0;
        SaveLoadController.SaveOut();
    }

    
    public void FakeSignalizationSucces()
    {
        audioSource.PlayOneShot(succesAudio);
        ViewHousStatusParticle(successParticle);
        EndRobbery();
        MainPlayer.Instance.Raiting = 25;
        
        AddScoreToBase();
        MainPlayer.Instance.Money = 25;
        TargetsManager.Instance.countUpdate++;
        if (TargetsManager.Instance.countUpdate >= 20)
        {
            AchivemntController.Instance.AchProtectedAbsolute();
        }
        SaveLoadController.SaveOut();
    }

    public void ViewHousStatusParticle(GameObject item) {
        StartCoroutine(ViewHousStatusCouratine(item));
    }

    IEnumerator ViewHousStatusCouratine(GameObject item) {
        item.SetActive(true);
        yield return new WaitForSeconds(2);
        item.SetActive(false);
    }


    public void ExpertDestroyAll() {
        OnCompleteRobbir?.Invoke();
        EndRobbery();

        MainPlayer.Instance.Message = "Эксперт прогнал грабителей";
        //Тут считаем очки, прибавляем или отнимаем и сколько
    }




}
