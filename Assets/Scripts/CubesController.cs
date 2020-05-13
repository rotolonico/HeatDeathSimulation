using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CubesController : MonoBehaviour
{
    public static Camera MainCamera;
    
    public static float ExchangeDecay;

    public GameObject cubePrefab;
    
    public Slider exchangeDecaySlider;
    public TextMeshProUGUI exchangeDecaySliderText;

    private void Awake() => MainCamera = Camera.main;

    private void Start() => exchangeDecaySlider.onValueChanged.AddListener(ChangeExchangeDecay);

    private void ChangeExchangeDecay(float value)
    {
        ExchangeDecay = value;
        exchangeDecaySliderText.text = $"%D = {Math.Round(value, 2)}";
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2)) SpawnCube();
        else if (Input.GetMouseButtonDown(0)) StartMovingCube();
        else if (Input.GetMouseButtonUp(0)) StopMovingCube();
        else if (Input.GetMouseButtonDown(1)) DestroyCube();
        else if (Math.Abs(Input.GetAxisRaw("Mouse ScrollWheel")) > 0.01f)
            SmudgeCubeTemperature(Input.GetAxisRaw("Mouse ScrollWheel") * 50);
    }

    private void SpawnCube()
    {
        var position = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        Instantiate(cubePrefab, position, Quaternion.identity);
    }

    private void StartMovingCube()
    {
        var cube = GetCube();
        if (cube == null) return;
        cube.StartMoving();
    }

    private void StopMovingCube()
    {
        var cube = GetCube();
        if (cube == null) return;
        cube.StopMoving();
    }

    private void SmudgeCubeTemperature(float smudge)
    {
        var cube = GetCube();
        if (cube == null) return;
        cube.SmudgeTemperature(smudge);
    }

    private void DestroyCube()
    {
        var cube = GetCube();
        if (cube == null) return;
        cube.Destroy();
    }

    private CubeController GetCube()
    {
        var hitObj = Physics2D.OverlapPoint(MainCamera.ScreenToWorldPoint(Input.mousePosition));
        if (hitObj == null || !hitObj.CompareTag("Cube")) return null;
        return hitObj.GetComponent<CubeController>();
    }
}