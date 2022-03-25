//@{
//    var count = 9;
//    var T = "";
//    var Tn = "";
//    for (int i = 0; i < count; i++)
//    {
//        T += "T" + (i + 1) + ",";
//    }
//    Tn = T + "JoinType";
//    T = T.TrimEnd(',');
//}
//public class Subqueryable<@T> : Subqueryable<T1> where T1 : class, new()
//{
//    public Subqueryable<@Tn> InnerJoin<JoinType>(Func<@Tn, bool> expression)
//    {
//        return new Subqueryable<@Tn>();
//    }
//    public Subqueryable<@Tn> LeftJoin<JoinType>(Func<@Tn, bool> expression)
//    {
//        return new Subqueryable<@Tn>();
//    }
//    @for(int i = 0; i<count; i++)
//        {
//            var itemcount = i + 1;

//    var wtn = "";

//            for (int j = 0; j<itemcount; j++)
//            {
//                wtn += "T" + (j+1) + ",";
//            }
//wtn = wtn.TrimEnd(',');
//            @:public @(i == 0 ? "new" : "") Subqueryable<@T> Where(Func<@wtn, bool> expression)
//            @:{
//                  @:return this;
//            @:}
            
//}
//}

