/**
 * Author: Yuri Fukuda 
 * created: June 16 2024
 **/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

namespace Airinterface.TicTacToe
{
    public class CellController : MonoBehaviour
    {
        private Button button;
        private TextMeshProUGUI label;



        public int row = 0;
        public int column = 0;

        public float duration = 0.1f;
        public AnimationCurve bounceCurve;


        private void Awake() {
            button = GetComponentInChildren<Button>();
            button.onClick.AddListener(OnClick);
            label = getText("Label");
        }


        public void enable()
        {
            Debug.Log($"#TTT cell{column},{row} enabled");
            button.interactable = true;            
        }

        public void disable()
        {
            button.interactable = false;
        }

        public void OnClick()
        {
            // get current type
            Debug.Log("Clicked");
            GameManager.Instance.click(this, NetworkManager.Instance.runner.LocalPlayer);
        }

        /* This is going to get called from Game Manager */
        public void clickExecute(TextType textType) {
            Debug.Log($"TTT clickExecute : {textType}");
            animate(textType);
        }

        private void animate(TextType textType) {
            setTextType(textType);
            StartCoroutine( bounceToPosition(label, 0, duration));
        }   

        private TextMeshProUGUI setText(string name)
        {
            Transform textTransform = button.transform.Find(name);
            if (textTransform != null)
            {
                return textTransform.GetComponent<TextMeshProUGUI>();
            }
            return null;
        }

        private TextMeshProUGUI getText(string name)
        {
            Transform textTransform = button.transform.Find(name);
            if (textTransform != null)
            {
                return textTransform.GetComponent<TextMeshProUGUI>();
            }
            return null;
        }

        private void setTextType(TextType textType)
        {
            if (label != null)
            {
                string labelText = textType == TextType.O ? "O" : "X";
                label.text = labelText;
                label.color = Color.black;
            }
        }

        private float CubicBezierInterpolation(float t, float p0, float p1, float p2, float p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            float result = uuu * p0; // (1 - t)^3 * P0
            result += 3 * uu * t * p1; // 3 * (1 - t)^2 * t * P1
            result += 3 * u * tt * p2; // 3 * (1 - t) * t^2 * P2
            result += ttt * p3; // t^3 * P3

            return result;
        }

        private IEnumerator bounceToPosition(TextMeshProUGUI text, float endZ, float duration)
        {
            Vector3 anchorPosition = text.GetComponent<RectTransform>().anchoredPosition3D;
            Vector3 targetPosition = new Vector3(anchorPosition.x, anchorPosition.y, endZ);


            float elapsed = 0.0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float normalizedT = CubicBezierInterpolation(t, 0.21f, -0.42f, 0.29f, 1.71f);
                float currentZ = Mathf.Lerp(anchorPosition.z, endZ, normalizedT);
                Vector3 currentPos = new Vector3(anchorPosition.x, anchorPosition.y, currentZ);
                text.GetComponent<RectTransform>().anchoredPosition3D = currentPos;
                yield return null;
            }

            // Ensure final scale matches the target
            text.GetComponent<RectTransform>().anchoredPosition3D = targetPosition;
        }


    }

    

}
