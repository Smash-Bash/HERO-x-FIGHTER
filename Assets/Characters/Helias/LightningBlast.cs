using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBlast : MonoBehaviour
{
    public LineRenderer line;
    public float lifetime;

    public void SetPositions(Vector3 start, Vector3 end)
    {
        transform.position = start;
        line.SetPosition(0, start);
        line.SetPosition(1, Vector3.Lerp(start, end, 0.25f) + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
        line.SetPosition(2, Vector3.Lerp(start, end, 0.5f) + new Vector3(Random.Range(-2f, 2f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
        line.SetPosition(3, Vector3.Lerp(start, end, 0.75f) + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
        line.SetPosition(4, end);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, line.GetPosition(0) + (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Time.deltaTime * 10));
        line.SetPosition(1, line.GetPosition(1) + (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Time.deltaTime * 10));
        line.SetPosition(2, line.GetPosition(2) + (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Time.deltaTime * 10));
        line.SetPosition(3, line.GetPosition(3) + (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Time.deltaTime * 10));
        line.SetPosition(4, line.GetPosition(4) + (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Time.deltaTime * 10));

        line.startColor = Color.Lerp(Color.clear, Color.yellow, lifetime);
        line.endColor = Color.Lerp(Color.clear, Color.yellow, lifetime);

        lifetime -= Time.deltaTime * 2;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
