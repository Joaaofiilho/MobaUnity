using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tags
{
    private Tags(string value) { Value = value; }

    public string Value { get; set; }

    public static Tags Player { get { return new Tags("Player"); } }
    public static Tags Map { get { return new Tags("Map"); } }
    public static Tags Minion { get { return new Tags("Minion"); } }
}
