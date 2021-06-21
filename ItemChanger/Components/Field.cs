using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Components
{
    public class Field<T> : MonoBehaviour
    {
        public T value;
        public string fieldName;
    }
}
