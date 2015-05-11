#r "packages/Aether/lib/net40/Aether.dll"
#r "packages/FParsec/lib/net40-client/FParsecCS.dll"
#r "packages/FParsec/lib/net40-client/FParsec.dll"
#r "packages/NUnit/lib/nunit.framework.dll"
#r "packages/Unquote/lib/net40/Unquote.dll"
#r "packages/FsCheck/lib/net45/FsCheck.dll"
//#r "packages/FSharp.Core/lib/net40/FSharp.Core.dll"

#load "paket-files/xyncro/chiron/src/Chiron/Chiron.fs"

// Existing tests
open Chiron
open NUnit.Framework
open Swensen.Unquote

let t1 =
    Object (Map.ofList
        [ "bool", Bool true
          "number", Number 2. ])

[<Test>]
let ``Json.parse returns correct values`` () =
    Json.parse "\"hello\"" =? String "hello"
    Json.parse "\"\"" =? String ""
    Json.parse "\"\\n\"" =? String "\n"
    Json.parse "\"\\u005c\"" =? String "\\"
    Json.parse "\"푟\"" =? String "푟"

[<Test>]
let ``Json.format returns correct values`` () =
    (* String *)
    Json.format <| String "hello" =? "\"hello\""

    (* Complex type *)
    Json.format t1 =? "{\"bool\":true,\"number\":2}"

    Json.format (String "hello") =? "\"hello\""
    Json.format (String "") =? "\"\""
    Json.format (String "푟") =? "\"푟\""
    Json.format (String "\t") =? "\"\t\""

``Json.parse returns correct values`` ()
``Json.format returns correct values`` ()

// Let's add our properties!
open FsCheck

let inline roundTrip (thing : 'a) : 'a =
  Json.serialize thing
  |> Json.format
  |> Json.parse
  |> Json.deserialize

type Properties =
  static member ``Strings can be round tripped`` (str : string) =
    roundTrip str = str
  (*
  static member ``Strings can be round tripped`` (str : NonNull<string>) =
    let s = str.Get
    roundTrip s = s
  *)

FsCheck.Check.All<Properties>(FsCheck.Config.Quick)
