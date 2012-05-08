﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CommandHandlerCodeIssue;
using Moq;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers.Common;
using Roslyn.Services;
using Roslyn.Services.Editor;
using Xunit;

namespace CommandHandlerCodeIssueTestFixture
{
    public class given_a_project
    {
        protected const string Code = @"
namespace TestCode 
{
    using System;  
    using MessagingToolsRoslynTest;
    using MessagingToolsRoslynTest.Interfaces;

    public interface ICommandHandler<T> where T : ICommand
    {
        void Handles(T command);
    }    
                                              
    public class FoomandHandler : ICommandHandler<Foo>
    {
        public bool WasCalled { get; private set; }
        public void Handles(Foo command)
        {
            WasCalled = true;
            Console.Write(""Foomand handled {0}"", command.Name);
        }
    }                                           
}";

        private readonly IDocument document;
        private readonly CodeIssueProvider sut;
        private readonly MockRepository mockRepos = new MockRepository(MockBehavior.Loose);
        public given_a_project()
        {
            var a = Workspace.LoadStandAloneProject(@"..\..\..\MessagingToolsRoslynTest\MessagingToolsRoslynTest.csproj");

            document = a.CurrentSolution.Projects.First().Documents.First(x => x.DisplayName == "TestDoc.cs");
            var mockEditFactory = mockRepos.Create<ICodeActionEditFactory>();
            sut = new CodeIssueProvider(mockEditFactory.Object);
        }

        [Fact]
        public void when_multiple_command_handlers_flags_issue()
        {
            var node = document.GetSyntaxTree().Root.DescendentNodes().OfType<ClassDeclarationSyntax>().First();
            var result = sut.GetIssues(document, node, new CancellationToken());

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Console.WriteLine(result.First().Description);
        }

        [Fact]
        public void when_handler_handles_multiple_different_commands_does_not_flag_as_issue()
        {
            
        }

    }
}