Namespace GridWithContextMenu.Data

    Public Class WeatherForecastService

        Public ReadOnly Summaries As String() = {"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"}

        Private Forecasts As List(Of WeatherForecast)?

        Public Function GetForecastAsync() As Task(Of List(Of WeatherForecast))
            If Forecasts Is Nothing Then
                Dim rnd = New Random()
                Forecasts = Enumerable.Range(1, 5).[Select](Function(index) New WeatherForecast With {.ID = index, .[Date] = DateTime.Today.AddDays(index), .TemperatureC = rnd.[Next](-20, 55), .Summary = Summaries(rnd.[Next](Summaries.Length))}).ToList()
            End If

            Return Task.FromResult(Forecasts)
        End Function

        Public Function GetSummaries() As String()
            Return Summaries
        End Function

        Public Function ChangeForecastAsync(ByVal changed As WeatherForecast) As Task(Of List(Of WeatherForecast))
            Dim orginalForecast = Forecasts.FirstOrDefault(Function(i) i.ID Is changed.ID)
            If orginalForecast IsNot Nothing Then
                orginalForecast.TemperatureC = changed.TemperatureC
                orginalForecast.Summary = changed.Summary
                orginalForecast.Date = changed.Date
            Else
                Forecasts.Add(changed)
            End If

            Return Task.FromResult(Forecasts)
        End Function

        Public Function Remove(ByVal forecast As WeatherForecast) As Task(Of List(Of WeatherForecast))
            Forecasts.Remove(forecast)
            Return Task.FromResult(Forecasts)
        End Function
    End Class
End Namespace
