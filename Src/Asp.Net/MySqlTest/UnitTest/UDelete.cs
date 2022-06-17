using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class UDelete
    {

        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
                DisableNvarchar = true
            };
            db.CodeFirst.InitTables<PlayerWeaponSkinEntity>();
            db.Deleteable(new List<PlayerWeaponSkinEntity>() {
             new PlayerWeaponSkinEntity(){
              PlayerId=1,
               Seed=1
             },
                new PlayerWeaponSkinEntity(){
              PlayerId=1,
               Seed=1
             }
            }).ExecuteCommand();
        }

        [SugarTable("kcf_player_weaponskin")]
        public class PlayerWeaponSkinEntity
        {
            /// <summary>
            /// 玩家PlayerID
            /// </summary>
            [SugarColumn(ColumnName = "pid", IsPrimaryKey = true)]
            public uint PlayerId { get; set; }

            [SugarColumn(ColumnName = "weapon", IsPrimaryKey = true)]
            public string WeaponClass { get; set; } = "__weapon_class__";

            public ushort Skin { get; set; }
            public ushort Seed { get; set; }
            public decimal Wear { get; set; } // don't use float
        }
    }
}
