using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LinhQ : MonoBehaviour
{
    public class Sinhvien
    {
        public string name;
        public int old;
        public Sinhvien(string name, int old)
        {
            this.name = name;
            this.old = old;
        }
    }
    public List<Sinhvien> lstSV = new List<Sinhvien>();
    public void Start()
    {
        Sinhvien sinhvien;
        sinhvien = new Sinhvien("Nam",24);
        lstSV.Add(sinhvien);
        sinhvien = new Sinhvien("Toan", 14);
        lstSV.Add(sinhvien);
        sinhvien = new Sinhvien("Phuong", 28);
        lstSV.Add(sinhvien);

        // tìm kiếm thông thường
        //for (int i = 0; i < lstSV.Count; i++)
        //{
        //    int oldsv = 14;
        //    if (oldsv == lstSV[i].old)
        //    {
        //        Debug.Log("Name :" + lstSV[i].name + "  " + "Old: " + lstSV[i].old);
        //    }
        //}
        //LinQ Lọc các phần tử mà old >=16
        //List<Sinhvien> FindSV = lstSV.Where(sv => sv.old >= 16 && sv.name.Contains("ng")).ToList();

        //List<Sinhvien> FindSV = lstSV.Where(sv => sv.old >= 1).OrderBy(sv => sv.old).Take(1).ToList();
        List<Sinhvien> FindSV = lstSV.Where(sv => sv.old >= 1).OrderBy(sv => sv.old).Skip(1).Take(1).ToList();

        foreach (Sinhvien s in FindSV)
        {
            Debug.Log(s.name + "  " + s.old);
        }
    }  

}
