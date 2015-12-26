(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,List,T,UI,Next,Client,Doc,AttrProxy,Attr,Concurrency,Remoting,AjaxRemotingProvider,Var1,TrafficSim,VisualTest,Client1,Jatek,GameModule,console,Collections,MapModule,Seq,SceneModule,VisualModule,Var;
 Runtime.Define(Global,{
  TrafficSim:{
   Client:{
    Main:function()
    {
     var action,arg10,b,ats,arg20,input,ch,arg101,output,arg201,arg202;
     action=function()
     {
     };
     arg10=Runtime.New(T,{
      $:0
     });
     b=Doc.Button("test",arg10,action);
     ats=List.ofArray([AttrProxy.Create("value","")]);
     arg20=Runtime.New(T,{
      $:0
     });
     input=Doc.Element("input",ats,arg20);
     ch=Runtime.New(T,{
      $:0
     });
     arg101=[];
     output=Doc.Element("h1",arg101,ch);
     arg202=Runtime.New(T,{
      $:0
     });
     arg201=List.ofArray([input,Doc.Element("button",List.ofArray([Attr.Handler("click",function()
     {
      return function()
      {
       var arg00;
       arg00=Concurrency.Delay(function()
       {
        return Concurrency.Bind(AjaxRemotingProvider.Async("GameUI:1",[input.GetValue()]),function(_arg11)
        {
         output.SetText(_arg11);
         return Concurrency.Return(null);
        });
       });
       return Concurrency.Start(arg00,{
        $:0
       });
      };
     })]),List.ofArray([Doc.TextNode("Send")])),Doc.Element("hr",[],arg202),Doc.Element("h4",List.ofArray([AttrProxy.Create("class","text-muted")]),List.ofArray([Doc.TextNode("The server responded:")])),Doc.Element("div",List.ofArray([AttrProxy.Create("class","jumbotron")]),List.ofArray([output]))]);
     return Doc.Element("div",[],arg201);
    },
    Start:function(input,k)
    {
     var arg00;
     arg00=Concurrency.Delay(function()
     {
      return Concurrency.Bind(AjaxRemotingProvider.Async("GameUI:1",[input]),function(_arg1)
      {
       return Concurrency.Return(k(_arg1));
      });
     });
     return Concurrency.Start(arg00,{
      $:0
     });
    }
   },
   VisualTest:{
    Client:{
     GetPlayerId:function()
     {
      var arg00;
      arg00=Concurrency.Delay(function()
      {
       return Concurrency.Bind(AjaxRemotingProvider.Async("GameUI:0",[]),function(_arg1)
       {
        Var1.Set(Client1.playerGuid(),_arg1);
        return Concurrency.Return(null);
       });
      });
      return Concurrency.Start(arg00,{
       $:0
      });
     },
     Main:function()
     {
      var arg20;
      GameModule.Create(400,400,function()
      {
       return console?console.log("test"):undefined;
      },MapModule.OfArray(Seq.toArray(List.ofArray([[1,SceneModule.Create(List.ofArray([VisualModule.Create("App_Themes/Standard/BlueHexagon.png",50,50,60,52)]))]]))),1);
      Client1.GetPlayerId();
      arg20=List.ofArray([Doc.Input(Runtime.New(T,{
       $:0
      }),Client1.playerGuid())]);
      return Doc.Element("div",[],arg20);
     },
     playerGuid:Runtime.Field(function()
     {
      return Var.Create("");
     })
    }
   }
  }
 });
 Runtime.OnInit(function()
 {
  List=Runtime.Safe(Global.WebSharper.List);
  T=Runtime.Safe(List.T);
  UI=Runtime.Safe(Global.WebSharper.UI);
  Next=Runtime.Safe(UI.Next);
  Client=Runtime.Safe(Next.Client);
  Doc=Runtime.Safe(Client.Doc);
  AttrProxy=Runtime.Safe(Client.AttrProxy);
  Attr=Runtime.Safe(Client.Attr);
  Concurrency=Runtime.Safe(Global.WebSharper.Concurrency);
  Remoting=Runtime.Safe(Global.WebSharper.Remoting);
  AjaxRemotingProvider=Runtime.Safe(Remoting.AjaxRemotingProvider);
  Var1=Runtime.Safe(Next.Var1);
  TrafficSim=Runtime.Safe(Global.TrafficSim);
  VisualTest=Runtime.Safe(TrafficSim.VisualTest);
  Client1=Runtime.Safe(VisualTest.Client);
  Jatek=Runtime.Safe(Global.WebSharper.Jatek);
  GameModule=Runtime.Safe(Jatek.GameModule);
  console=Runtime.Safe(Global.console);
  Collections=Runtime.Safe(Global.WebSharper.Collections);
  MapModule=Runtime.Safe(Collections.MapModule);
  Seq=Runtime.Safe(Global.WebSharper.Seq);
  SceneModule=Runtime.Safe(Jatek.SceneModule);
  VisualModule=Runtime.Safe(Jatek.VisualModule);
  return Var=Runtime.Safe(Next.Var);
 });
 Runtime.OnLoad(function()
 {
  Client1.playerGuid();
  return;
 });
}());
