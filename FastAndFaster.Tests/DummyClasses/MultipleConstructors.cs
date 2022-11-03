namespace FastAndFaster.Tests.DummyClasses;

public class MultipleConstructors
{
    public int Age { get; private set; }

    public string Name { get; private set; }

    public List<bool> Bools { get; private set; }

    public MultipleConstructors()
    {
    }

    public MultipleConstructors(int age, string name, List<bool> bools)
    {
        Age = age;
        Name = name;
        Bools = bools;
    }

    protected MultipleConstructors(int age)
    {
        Age = age;
    }
}
