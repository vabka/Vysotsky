module FSharpProd.HttpTests.SimpleHttpTest

open System.Net
open System.Net.Http
open System.Text
open System.Threading.Tasks
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Http.FSharp

// in this example we use:
// - NBomber.Http (https://nbomber.com/docs/plugins-http)

let step =
    HttpStep.create
        ("get_auth_token",
         (fun context ->
             Http.createRequest "POST" "http://localhost:5001/api/auth/authenticate"
             |> Http.withBody (new StringContent("{ \"username\": \"admin\", \"password\": \"admin\" }", Encoding.UTF8, "application/json"))
             |> Http.withHeader "Accept" "application/json"
             |> Http.withCheck (fun m ->
                 if m.StatusCode = HttpStatusCode.OK then
                     Task.FromResult(Response.Ok())
                 else
                     Task.FromResult(Response.Fail()))))



Scenario.create "simple_http" [ step ]
|> Scenario.withWarmUpDuration (seconds 5)
|> Scenario.withLoadSimulations [ InjectPerSec(rate = 100, during = seconds 30) ]
|> NBomberRunner.registerScenario
|> NBomberRunner.withTestSuite "http"
|> NBomberRunner.withTestName "simple_test"
|> NBomberRunner.run
|> ignore
