using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    [Header("摆动位置")]
    public float pOne;
    public float pTwo;
    public float _t;
    bool flag = false;
    Vector2 init = new Vector2();
    Vector2 aim = new Vector2();
    float t = 0;
    // Start is called before the first frame update
    void Start()
    {
        aim = new Vector2(pTwo, this.transform.localPosition.y);
        init = new Vector2(pOne, this.transform.localPosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t > _t)
        {
            if (this.transform.localPosition.x <= pOne)
            {
                aim = new Vector2(pTwo, this.transform.localPosition.y);
                init = new Vector2(pOne, this.transform.localPosition.y);
                t = 0;

            }
            else if (this.transform.localPosition.x >= pTwo)
            {
                aim = new Vector2(pOne, this.transform.localPosition.y);
                init = new Vector2(pTwo, this.transform.localPosition.y);
                t = 0;

            }
        }
        this.transform.localPosition = Vector2.Lerp(init, aim, t / _t);
    }
}
