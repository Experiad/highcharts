#load "tools/includes.fsx"
open IntelliFactory.Build

open System.IO
let ( +/ ) a b = Path.Combine(a, b)

let bt = 
    BuildTool().VersionFrom("WebSharper")
        .WithFSharpVersion(FSharpVersion.FSharp30)
        .WithFramework(fun fw -> fw.Net40)

let tempDir = __SOURCE_DIRECTORY__ +/ ".temp"

//Directory.CreateDirectory tempDir |> ignore

//do  use cl = new System.Net.WebClient()
//    let download (url: string) name =
//        try cl.DownloadFile(url, tempDir +/ name)
//        with _ -> failwithf "Failed to download %s" url 
//
//    download "http://api.highcharts.com/highcharts/option/dump.json" "hcconfigs.json"
//    download "http://api.highcharts.com/highcharts/object/dump.json" "hcobjects.json"
//    download "http://api.highcharts.com/highstock/option/dump.json" "hsconfigs.json"
//    download "http://api.highcharts.com/highstock/object/dump.json" "hsobjects.json"
//    download "http://api.highcharts.com/highmaps/option/dump.json" "hmconfigs.json"
//    download "http://api.highcharts.com/highmaps/object/dump.json" "hmobjects.json"

let common =
    bt.WebSharper4.Library("HighchartsGeneratorCommon")
        .SourcesFromProject()
        .References(fun r -> [r.NuGet("FParsec").Reference()])

let hc =
    bt.WebSharper4.Extension("WebSharper.Highcharts")
        .SourcesFromProject()
        .References(fun r -> [r.Project common; r.NuGet("FParsec").Reference()])

let hs =
    bt.WebSharper4.Extension("WebSharper.Highstock")
        .SourcesFromProject()
        .References(fun r -> [r.Project common; r.NuGet("FParsec").Reference()])

let hm =
    bt.WebSharper4.Extension("WebSharper.Highmaps")
        .SourcesFromProject()
        .References(fun r -> [r.Project common; r.NuGet("FParsec").Reference(); r.Project hc; r.Project hs])

bt.Solution [
    common
    hc
    hs
    hm

    bt.PackageId("WebSharper.Highcharts").NuGet.CreatePackage()
        .Description("WebSharper bindings to Highcharts")
        .Add(hc)
        .Configure(fun c ->
            { c with
                Authors = ["IntelliFactory"]
                Id = "WebSharper.Highcharts"
                Title = Some ("WebSharper.Highcharts")
                NuGetReferences =
                    c.NuGetReferences |> List.filter (fun dep -> 
                        dep.PackageId.Contains "FParsec" |> not
                    )
            })
    bt.PackageId("WebSharper.Highstock").NuGet.CreatePackage()
        .Description("WebSharper bindings to Highstock")
        .Add(hs)
        .Configure(fun c ->
            { c with
                Authors = ["IntelliFactory"]
                Id = "WebSharper.Highstock"
                Title = Some ("WebSharper.Highstock")
                NuGetReferences =
                    c.NuGetReferences |> List.filter (fun dep -> 
                        dep.PackageId.Contains "FParsec" |> not
                    )
            })
    bt.PackageId("WebSharper.Highmaps").NuGet.CreatePackage()
        .Description("WebSharper bindings to Highmaps")
        .Add(hm)
        .Configure(fun c ->
            { c with
                Authors = ["IntelliFactory"]
                Id = "WebSharper.Highmaps"
                Title = Some ("WebSharper.Highmaps")
                NuGetReferences =
                    c.NuGetReferences |> List.filter (fun dep -> 
                        dep.PackageId.Contains "FParsec" |> not
                    )
            })
]
|> bt.Dispatch
