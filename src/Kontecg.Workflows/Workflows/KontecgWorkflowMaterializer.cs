using Kontecg.Dependency;
using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Entities;

namespace Kontecg.Workflows
{
    /// <summary>
    /// Provides context for the Kontecg workflow materializer.
    /// </summary>
    /// <param name="WorkflowBuilderType">The type of the workflow builder.</param>
    public record KontecgWorkflowMaterializerContext(Type WorkflowBuilderType);

    public class KontecgWorkflowMaterializer : IWorkflowMaterializer
    {
        private readonly IWorkflowBuilderFactory _workflowBuilderFactory;
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// The name of the materializer.
        /// </summary>
        public const string MaterializerName = "KONTECG";

        public KontecgWorkflowMaterializer(IWorkflowBuilderFactory workflowBuilderFactory, IIocResolver iocResolver)
        {
            _workflowBuilderFactory = workflowBuilderFactory;
            _iocResolver = iocResolver;
        }

        public async ValueTask<Workflow> MaterializeAsync(WorkflowDefinition definition, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public string Name => MaterializerName;
    }
}
