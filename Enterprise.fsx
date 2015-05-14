#r "System.Xml"
#r "System.Xml.Linq"
#r "packages/FsCheck/lib/net45/FsCheck.dll"

open System
open System.Xml.Linq

type AddCommand =
  {
    Guid : Guid
    Content : string
  }

let add doc command =
  let xdoc = XDocument.Parse doc
  let node = XElement(XName.Get "Enhancement", command.Content)
  node.SetAttributeValue(XName.Get "Command", command.Guid)
  xdoc.Root.Add node
  xdoc.ToString()

(*
let add doc command =
  let xdoc = XDocument.Parse doc
  let enhancements = xdoc.Descendants(XName.Get "Enhancement")
  let exists =
    enhancements
    |> Seq.exists (fun x -> Guid(x.Attribute(XName.Get "Command").Value) = command.Guid)
  if exists then
    doc
  else
    let node = XElement(XName.Get "Enhancement", command.Content)
    node.SetAttributeValue(XName.Get "Command", command.Guid)
    xdoc.Root.Add node
    xdoc.ToString()
*)


// Testing!
open FsCheck

let empty = "<root />"

type Properties =
  static member ``Add commands should be idempotent`` command =
    let doc1 = add empty command
    let doc2 = add (add empty command) command
    doc1 = doc2
  (*
  static member ``It doesn't crash`` command =
    add empty command
    true
  *)
  (*
  static member ``Add commands should never reduce the document size`` commands =
    let rec bigger doc commands =
      match commands with
      | [] -> true
      | h::t ->
        let currentSize = (XDocument.Parse doc).Descendants() |> Seq.length
        let newDoc = add doc h
        let newSize = (XDocument.Parse doc).Descendants() |> Seq.length
        if newSize < currentSize then
          false
        else
          bigger newDoc t
    bigger empty commands
  *)
  (*
  static member ``Add command actually adds a node`` command =
    let doc = XDocument.Parse (add empty command)
    let nodes =
      doc.Descendants(XName.Get "Enhancement")
    if Seq.length nodes <> 1 then
      false
    else
      nodes
      |> Seq.head
      |> fun node ->
            node.Value = command.Content
  *)

FsCheck.Check.All<Properties>(FsCheck.Config.Verbose)

