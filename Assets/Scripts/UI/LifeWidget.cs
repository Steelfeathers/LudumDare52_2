using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare52_2
{
    public class LifeWidget : MonoBehaviour
    {
        [SerializeField] private GameObject iconFull;

        private void Start()
        {
            iconFull.SetActive(true);
        }

        public void SetFull(bool isFull)
        {
            iconFull.SetActive(isFull);
        }
    }
}