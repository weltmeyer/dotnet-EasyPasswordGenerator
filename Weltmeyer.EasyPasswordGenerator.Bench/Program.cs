using Weltmeyer.EasyPasswordGenerator;
using Weltmeyer.EasyPasswordGenerator.Bench;

BenchmarkDotNet.Running.BenchmarkRunner.Run<Benchmarks>();

//var bench = new Benchmarks();
/*

bench.MinLength = 100;
for(int i=0;i<10000;i++)
    bench.CreatePasswordAndValidate();
    */
/*
for (int i = 0; i < 10000; i++)
{
    var pw=PasswordGenerator.Generate(bench._testConfig);
    Console.WriteLine(pw);
}*/