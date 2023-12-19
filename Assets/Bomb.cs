using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{

    public int seed;
    public GameObject[] segmentsPrefab;
    public GameObject timerPrefab;
    public Text bombName;
    private static readonly string[] letters = { "B","R","Z","H","Q","L","F","D" };
    private static readonly string[] numbers = { "1","2","3","5","6","7","8","9" };
    private static readonly string[] colors = { "Red", "Green", "Blue", "Yellow" };
    private List<Segment> segments;

    public AudioSource source;
    public AudioClip win, lose;
    private bool iswin, isfail;

    private int blocksCount;
    private int colorCount;
    private int letterCount;
    private int numberCount;
    private int powerCount;

    public int disablesCount;

    private string bombname;


    void Start()
    {
        segments = new List<Segment>();
        seed = Random.Range(0, int.MaxValue);
        Random.seed = seed;
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < Random.Range(3, 7); ++i)
        {
            builder.Append(Random.Range(0, 5) != 0 ? letters[Random.Range(0, letters.Length)] : numbers[Random.Range(0, numbers.Length)]);
        }
        builder.Append('-');
        for (int i = 0; i < Random.Range(2, 5); ++i)
        {
            builder.Append(Random.Range(0, 2) == 0 ? letters[Random.Range(0, letters.Length)] : numbers[Random.Range(0, numbers.Length)]);
        }
        bombname = builder.ToString();
        bombName.text = bombname;
        bombname = bombname.Replace("-", "");

        int blockCount = Random.Range(4, 13);
        blocksCount = blockCount;
        bool timerplaced=false;
        for (int i = 0; i < blockCount; ++i)
        {
            if (!timerplaced && (Random.Range(0, 5) == 0 || i == blockCount - 1))
            {
                timerplaced = true;
                float x = .712f;
                float y = 1.663f - ((i % 6) / 3) * 2.353f;//-.69
                float z = 2.2f - (i % 3) * 2.2f;//-2.2
                GameObject ob = Instantiate(timerPrefab, transform);
                if (i > 5)
                {
                    x = -x;
                    ob.transform.Rotate(0, 180, 0);
                }
                ob.transform.localPosition = new Vector3(x, y, z);
                ob.GetComponent<Segment>().time = 60 * blockCount;
                ob.GetComponent<Segment>().defactotime = ob.GetComponent<Segment>().time;
                ob.GetComponent<Segment>().bomb = this;
                segments.Add(ob.GetComponent<Segment>());
            }
            else
            {
                BlockType blockType = (BlockType)Random.Range(0, 3);

                switch (blockType)
                {
                    case BlockType.Numeric:
                        ++numberCount;
                        break;
                    case BlockType.Coloric:
                        ++colorCount;
                        break;
                    case BlockType.Letteric:
                        ++letterCount;
                        break;
                }

                float x = .712f;
                float y = 1.663f - ((i % 6) / 3) * 2.353f;//-.69
                float z = 2.2f - (i % 3) * 2.2f;//-2.2

                GameObject ob = Instantiate(segmentsPrefab[(int)blockType], transform);

                if (i > 5)
                {
                    x = -x;
                    ob.transform.Rotate(0, 180, 0);
                }
                ob.transform.localPosition = new Vector3(x, y, z);
                ob.GetComponent<Segment>().blockType = blockType;
                ob.GetComponent<Segment>().bomb = this;
                segments.Add(ob.GetComponent<Segment>());
            }
        }
        for (int i = 0; i < blockCount; ++i) if(!segments[i].istimer)
        {
            BlockType blockType = segments[i].blockType;
            string passcode = "";

            switch (blockType)
            {
                case BlockType.Numeric:
                        passcode = GenerateNumeric(i);
                    break;
                case BlockType.Coloric:
                        passcode = GenerateColor(i);
                    
                    break;
                case BlockType.Letteric:
                    passcode = GenerateLetteric(i);
                    break;
            }
            segments[i].Answer = passcode.Replace("-", "");
            print($"Block {i}: {blockType}, Password = {passcode}");
        }
    }
    private string GenerateLetteric(int id) 
    {
        string passcode =
            letters[(id) % letters.Length] + "-" +
            letters[(letterCount) % letters.Length] + "-"+
            (Contains(letters, bombname[2].ToString()) ? bombname[2].ToString() : letters[int.Parse(bombname[2].ToString()) % letters.Length]) + "-"+
            letters[(bombname.Length+id)%letters.Length] +"-"+
            (Contains(letters, bombname[bombname.Length-1].ToString()) ? bombname[bombname.Length - 1].ToString() : letters[int.Parse(bombname[bombname.Length - 1].ToString()) % letters.Length]) + "-" +
            letters[(id*letterCount*11)%letters.Length];
        return passcode;
    }
    private string GenerateColor(int id) 
    {
        string passcode =
            colors[(id + blocksCount) % colors.Length] + "-" +
            (powerCount > 2 ? colors[(letterCount) % colors.Length] : colors[(colorCount) % colors.Length]) + "-" +
            (Contains(numbers, bombname[1].ToString()) ? colors[int.Parse(bombname[1].ToString()) % colors.Length]:colors[(id+colorCount)%colors.Length])+"-"+
            (bombname[0] == 'D'?colors[0]:colors[id%colors.Length])
           ;
        return passcode;
    }
    private string GenerateNumeric(int id) 
    {
        string passcode =
            GetNumber((id + blocksCount) % 10) + "-" +
            GetNumber((id + numberCount) % 10) + "-" +
            GetNumber(NumInBomb() % 10) + "-" +
            GetNumber((numberCount*931) % 10);
        return passcode;
    }
    private string GetNumber(int num) 
    {
        if (num == 4) { return "5"; }
        if (num == 0) { return "1"; }
        return num.ToString();
    }
    private int NumInBomb() 
    {
        for (int i = 0; i < bombname.Length; ++i) 
        {
            if (char.IsDigit(bombname[i])) { return int.Parse(bombname[i].ToString()); }
        }
        return bombname.Length + 1;
    }
    private bool Contains(string[] arr,string prediction) 
    {
        for (int i = 0; i < arr.Length; ++i)
        {
            if (arr[i] == prediction) { return true; }
        }
        return false;
    }

    Quaternion target;

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.F5)) 
        {
            Application.LoadLevel(0);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            Application.Quit();
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * 3);
        if (iswin||isfail) return;
        if (Input.GetKey(KeyCode.Mouse1)) 
        {
            transform.Rotate(0, -Input.GetAxis("Mouse X")*3, Input.GetAxis("Mouse Y") * 3,Space.World);
            target = transform.rotation;
        }
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            target = Quaternion.identity;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 99)) 
            {
                if (hit.transform.GetComponent<Segment>() != null) 
                {
             //       Debug.Log(hit.collider.name);
                    hit.transform.GetComponent<Segment>().Add(hit.collider.name);
                }
            }
        }
        bool solved=true;
        bool failed=false;
        for (int i = 0; i < segments.Count; ++i)
        {
            solved = solved & (segments[i].Solved | segments[i].istimer);
            failed = failed | segments[i].failed;
        }
        if (solved) {
            source.clip = win;
            source.Play();
            iswin = true;
            target = Quaternion.identity;
            for (int i = 0; i < segments.Count; ++i)
            {
                segments[i].istimer = false;
            }
        }
        if (failed)
        {
            source.clip = lose;
            source.Play();
            isfail = true;
            for (int i = 0; i < segments.Count; ++i)
            {
                segments[i].istimer = false;
            }
        }
    }
}
public enum BlockType 
{
    Numeric,
    Coloric,
    Letteric
}
