(**
// can't yet format YamlFrontmatter (["category: Utilities"; "categoryindex: 1"; "index: 3"], Some { StartLine = 2 StartColumn = 0 EndLine = 5 EndColumn = 8 }) to pynb markdown

*)
#r "nuget: FSharp.Data,4.1.1"
(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/diffsharp/diffsharp.github.io/master?filepath=library/HtmlParser.ipynb)&emsp;
[![Script](../img/badge-script.svg)](library/HtmlParser.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)](library/HtmlParser.ipynb)

# HTML Parser

This article demonstrates how to use the HTML Parser to parse HTML files.

The HTML parser takes any fragment of HTML, uri or a stream and trys to parse it into a DOM. 
The parser is based on the [HTML Living Standard](http://www.whatwg.org/specs/web-apps/current-work/multipage/index.html#contents)
Once a document/fragment has been parsed, a set of extension methods over the HTML DOM elements allow you to extract information from a web page
independently of the actual HTML Type provider.
*)
open FSharp.Data
(**
The following example uses Google to search for `FSharp.Data` then parses the first set of
search results from the page, extracting the URL and Title of the link.
We use the `cref:T:FSharp.Data.HtmlDocument` type.

To achieve this we must first parse the webpage into our DOM. We can do this using
the `cref:M:FSharp.Data.HtmlDocument.Load` method. This method will take a URL and make a synchronous web call
to extract the data from the page. Note: an asynchronous variant `cref:M:FSharp.Data.HtmlDocument.AsyncLoad` is also available  
*)
let results = HtmlDocument.Load("http://www.google.co.uk/search?q=FSharp.Data")
(**
Now that we have a loaded HTML document we can begin to extract data from it. 
Firstly we want to extract all of the anchor tags `a` out of the document, then
inspect the links to see if it has a `href` attribute, using `cref:M:FSharp.Data.HtmlDocumentExtensions.Descendants`. If it does, extract the value,
which in this case is the url that the search result is pointing to, and additionally the 
`InnerText` of the anchor tag to provide the name of the web page for the search result
we are looking at. 
*)
let links = 
    results.Descendants ["a"]
    |> Seq.choose (fun x -> 
           x.TryGetAttribute("href")
           |> Option.map (fun a -> x.InnerText(), a.Value())
    )
    |> Seq.toList
(**
Now that we have extracted our search results you will notice that there are lots of
other links to various Google services and cached/similar results. Ideally we would 
like to filter these results as we are probably not interested in them.
At this point we simply have a sequence of Tuples, so F# makes this trivial using `Seq.filter`
and `Seq.map`.
*)
let searchResults =
    links
    |> List.filter (fun (name, url) -> 
                    name <> "Cached" && name <> "Similar" && url.StartsWith("/url?"))
    |> List.map (fun (name, url) -> name, url.Substring(0, url.IndexOf("&sa=")).Replace("/url?q=", ""))
(**
Putting this all together yields the following:
```
[("fsprojects/FSharp.Data: F# Data: Library for Data Access - GitHubgithub.com › fsharp › FSharp",
  "https://github.com/fsharp/FSharp.Data");
 ("FSharp.Data: Data Access Made Simplefsprojects.github.io › FSharp",
  "http://fsprojects.github.io/FSharp.Data/");
 ("FSharp.Data.TypeProvidersfsprojects.github.io › FSharp.Data.TypeProviders",
  "https://fsprojects.github.io/FSharp.Data.TypeProviders/");
 ("FSharp.Data 4.1.0 - NuGet Gallerywww.nuget.org › packages › FSharp",
  "https://www.nuget.org/packages/FSharp.Data/");
 ("Guide - Data Access | The F# Software Foundationfsharp.org › guides › data-access",
  "https://fsharp.org/guides/data-access/");
 ("Working with data frames in F# - FsLabfslab.org › Deedle › frame",
  "https://fslab.org/Deedle/frame.html");
 ("FSharp.Data (@FSharpData) | Twittertwitter.com › fsharpdata",
  "https://twitter.com/fsharpdata");
 ("Is the FSharp.Data XML type provider generative or not? - Stack ...stackoverflow.com › questions › is-the-fsharp-data-xml-type-provider-gen...",
  "https://stackoverflow.com/questions/65465265/is-the-fsharp-data-xml-type-provider-generative-or-not");
 ("F# Data: New type provider library - Tomas Petricektomasp.net › blog › fsharp-data",
  "http://tomasp.net/blog/fsharp-data.aspx/");
 ("Introduction to F# Type Providers | TheSharperDevthesharperdev.com › introduction-to-fsharp-type-providers",
  "https://thesharperdev.com/introduction-to-fsharp-type-providers/");
 ("Learn more",
  "http://support.google.com/websearch%3Fp%3Dws_settings_location%26hl%3Den");
 ("Sign in",
  "https://accounts.google.com/ServiceLogin%3Fcontinue%3Dhttp://www.google.co.uk/search%253Fq%253DFSharp.Data%26hl%3Den")]
```

*)

