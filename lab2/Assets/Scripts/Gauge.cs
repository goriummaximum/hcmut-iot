using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Gauge : MonoBehaviour
{
    public Transform needleTransform;
    public float rotateTweenTime = 0.75f;
    
    public float MAX_VALUE_ANGLE = 195;
    public float MIN_VALUE_ANGLE = -25;
    private float totalAngle;

    public Transform valueLabelTemplateTransform;
    public float MAX_VALUE_LABEL_ANGLE = -45;
    public float MIN_VALUE_LABEL_ANGLE = 225;
    public float totalAngleLabel;
    public float LabelsTotal = 10;

    public Text valueText;

    public float valueMax;
    public float valueMin;
    public float value;

    // Start is called before the first frame update
    void Awake()
    {
        totalAngle = Mathf.Abs(MAX_VALUE_ANGLE - MIN_VALUE_ANGLE);
        totalAngleLabel = Mathf.Abs(MAX_VALUE_LABEL_ANGLE - MIN_VALUE_LABEL_ANGLE);

        CreateValueLabels();
        valueLabelTemplateTransform.gameObject.SetActive(false); //hind the label template
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetValue(float value, string unit) {
        this.value = value;
        if (this.value > valueMax) this.value = valueMax;
        if (this.value < valueMin) this.value = valueMin;
        valueText.text = string.Format("{0}{1}", this.value, unit);
        //needleTransform.eulerAngles = new Vector3(0, 0, GetValueRotation());
        needleTransform.DORotate(new Vector3(0, 0, GetValueRotation()), rotateTweenTime);
    } 

    public float GetValueRotation() {
        return MIN_VALUE_ANGLE - ((value - valueMin) / (valueMax - valueMin)) * totalAngle;
    }


    public void CreateValueLabels() {
        for (int i = 0; i <= LabelsTotal; i++) {
            Transform valueLabelTransform = Instantiate(valueLabelTemplateTransform, transform);
            float valueLabelRatio = (float)i / LabelsTotal;
            float valueLabelAngle = MIN_VALUE_LABEL_ANGLE - valueLabelRatio * totalAngleLabel;
            valueLabelTransform.eulerAngles = new Vector3(0, 0, valueLabelAngle);
            valueLabelTransform.Find("ValueText").GetComponent<Text>().text = Mathf.RoundToInt(valueMin + valueLabelRatio * (valueMax - valueMin)).ToString();
            valueLabelTransform.Find("ValueText").eulerAngles = Vector3.zero;
            Debug.Log(valueLabelRatio * valueMax);
        }
    }
}
