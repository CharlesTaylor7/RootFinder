namespace RootFinder.Tests

open RootFinder
open Solver
open FsCheck
open FsCheck.NUnit
open FsUnitTyped
open System.Linq

type FSCheckTests () =

  static do Arb.register<Generators>()

  [<Property>]
  static member ``Solve Random Polynomial`` (p: Polynomial) =

    let roots = solve p

    roots |> shouldHaveLength p.degree

    for i = 0 to roots.Length - 1 do
      p.eval(roots.[i]) |> shouldEqual Complex.zero

  [<Property>]
  static member ``Trace Random Polynomials`` (p: Polynomial) =

    solve_with_counts p |> Array.map (snd >> printfn "%O") |> ignore

  // Ignored for now
  // Polynomial product propagates too much error for even small degree polynomials.
//  [<Property>]
  static member ``Reconstruct Random Polynomial`` (p: Polynomial) =
    let roots = solve p
    let one = Polynomial [| Complex.one |]
    let product = Seq.fold (fun product root -> product * (Polynomial [| -root; Complex.one |])) one roots
    let scale = p.leadCoefficient

    scale * product |> shouldEqual p
