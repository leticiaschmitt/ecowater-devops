// Os testes de integração compartilham o mesmo banco SQL Server e aplicam as
// migrations ao subir a aplicação; rodar em sequência evita concorrência na criação do schema.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
