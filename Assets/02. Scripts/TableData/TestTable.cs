using UnityEngine;
using System.Collections.Generic;

namespace Core.Scripts.Table
{
    [System.Serializable]
    public class TestTableData
    {
        [SerializeField]
		private int _ID;
		public int ID
		{
			get { return _ID;}
			set { _ID = value;}
		}
		[SerializeField]
		private string _Name;
		public string Name
		{
			get { return _Name;}
			set { _Name = value;}
		}
		[SerializeField]
		private int _HP;
		public int HP
		{
			get { return _HP;}
			set { _HP = value;}
		}
		[SerializeField]
		private int _Damage;
		public int Damage
		{
			get { return _Damage;}
			set { _Damage = value;}
		}

    }

    [System.Serializable]
    public class TestTable : Table<TestTableData, int>
    {
    }
}