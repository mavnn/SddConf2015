#r "packages/FsCheck/lib/net45/FsCheck.dll"
#r "packages/FParsec/lib/net40-client/FParsecCS.dll"
#r "packages/FParsec/lib/net40-client/FParsec.dll"
#r "packages/NUnit/lib/nunit.framework.dll"
#r "packages/Unquote/lib/net40/Unquote.dll"
#r "packages/Aether/lib/net40/Aether.dll"

#load "paket-files/xyncro/chiron/src/Chiron/Chiron.fs"

// Existing tests
open Chiron
open NUnit.Framework
open Swensen.Unquote

let t1 =
    Json.Object (Map.ofList
        [ "bool", Bool true
          "number", Number 2. ])

[<Test>]
let ``Json.parse returns correct values`` () =
    Json.parse "\"hello\"" =? Json.String "hello"
    Json.parse "\"\"" =? Json.String ""
    Json.parse "\"\\n\"" =? Json.String "\n"
    Json.parse "\"\\u005c\"" =? Json.String "\\"
    Json.parse "\"푟\"" =? Json.String "푟"

[<Test>]
let ``Json.format returns correct values`` () =
    (* String *)
    Json.format <| Json.String "hello" =? "\"hello\""

    (* Complex type *)
    Json.format t1 =? "{\"bool\":true,\"number\":2}"

    Json.format (Json.String "hello") =? "\"hello\""
    Json.format (Json.String "") =? "\"\""
    Json.format (Json.String "푟") =? "\"푟\""
    Json.format (Json.String "\t") =? "\"\t\""

``Json.parse returns correct values`` ()
``Json.format returns correct values`` ()

// Let's add our properties!
open FsCheck
open Chiron.Operators

let inline roundTrip (thing : 'a) : 'a =
  Json.serialize thing
  |> Json.format
  |> Json.parse
  |> Json.deserialize

(*
type TestRecord =
    {
        StringField : string
        GuidField : System.Guid
        NumberField : int
    }
    static member FromJson (_ : TestRecord) =
            fun s g d -> { StringField = s; GuidField = g; NumberField = d }
        <!> Json.read "stringField"
        <*> Json.read "guidField"
        <*> Json.read "numberField"
    static member ToJson { StringField = s; GuidField = g; NumberField = d } =
        Json.write "stringField" s
        *> Json.write "guidField" g
        *> Json.write "numberField" d
*)

type Properties =
  static member ``Strings can be round tripped`` (str : string) =
    roundTrip str = str
  (*
  static member ``Strings can be round tripped`` (str : NonNull<string>) =
    let s = str.Get
    roundTrip s = s
  *)
  (*
  static member ``Records can be round tripped`` (t : TestRecord) =
    roundTrip t = t
  *)

FsCheck.Check.All<Properties>(FsCheck.Config.Quick)

(*
type MyArbs =
  static member String () =
    FsCheck.Arb.Default.NonNull()
    |> FsCheck.Arb.convert
        (fun nonNull -> nonNull.Get)
        (fun (str : string) -> FsCheck.NonNull str)

FsCheck.Check.All<Properties>({ FsCheck.Config.Quick with Arbitrary = [typeof<MyArbs>] })
*)
