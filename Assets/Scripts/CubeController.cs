using System;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CubeController : MonoBehaviour
{
    public float exchangeSpeed;

    private Light2D light2D;
    private Transform thisTransform;
    private Transform textTransform;
    private TextMeshPro text;
    private Rigidbody2D rigidBody;

    private float temperature;
    private bool moving;

    private void Start()
    {
        light2D = GetComponent<Light2D>();
        thisTransform = transform;
        rigidBody = GetComponent<Rigidbody2D>();

        textTransform = thisTransform.GetChild(0);
        text = textTransform.GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        textTransform.localEulerAngles = new Vector3(0, 0, 360 - thisTransform.eulerAngles.z);
        textTransform.position = thisTransform.position + new Vector3(0, 1, 0);
        if (!moving) return;
        var thisTransformPosition = CubesController.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        thisTransformPosition.x = Mathf.Clamp(thisTransformPosition.x, -8, 8);
        thisTransformPosition.y = Mathf.Clamp(thisTransformPosition.y, -2.5f, 5);
        thisTransformPosition.z = 0;
        thisTransform.position = thisTransformPosition;
        var thisTransformRotation = thisTransform.eulerAngles;
        thisTransformRotation += new Vector3(0, 0, Time.deltaTime * 100);
        thisTransform.eulerAngles = thisTransformRotation;
        rigidBody.velocity = Vector2.zero;
    }

    public void StartMoving() => moving = true;

    public void StopMoving() => moving = false;

    public void Destroy() => Destroy(gameObject);

    public void SmudgeTemperature(float smudge)
    {
        temperature += smudge;
        temperature = Mathf.Clamp(temperature, 0, 500);
        UpdateTemperature();
    }

    public void ChangeTemperature(float value)
    {
        temperature = Mathf.Clamp(value, 0, 500);
        UpdateTemperature();
    }

    private void UpdateTemperature()
    {
        light2D.color = Color.Lerp(Color.white, Color.red, temperature / 500);
        text.text = $"T = {Math.Round(temperature)}K";
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!other.collider.CompareTag("Cube")) return;
        var otherController = other.collider.GetComponent<CubeController>();
        if (Math.Abs(otherController.temperature - temperature) < 0.5f) return;
        if (otherController.temperature > temperature)
        {
            otherController.SmudgeTemperature(-exchangeSpeed * Time.deltaTime);
            SmudgeTemperature(exchangeSpeed * Time.deltaTime * (1 - CubesController.ExchangeDecay));
        }
        else
        {
            otherController.SmudgeTemperature(exchangeSpeed * Time.deltaTime * (1 - CubesController.ExchangeDecay));
            SmudgeTemperature(-exchangeSpeed * Time.deltaTime);
        }
    }
}