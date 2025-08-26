using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Workflows;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Options;
using Kontecg.Dependency;

namespace Kontecg.Workflows
{
    public class KontecgWorkflowProvider : IWorkflowsProvider
    {
        private readonly IWorkflowBuilderFactory _workflowBuilderFactory;
        private readonly IIocResolver _iocResolver;
        private readonly RuntimeOptions _options;

        public KontecgWorkflowProvider(IWorkflowBuilderFactory workflowBuilderFactory, IIocResolver iocResolver, RuntimeOptions options)
        {
            _workflowBuilderFactory = workflowBuilderFactory;
            _iocResolver = iocResolver;
            _options = options;
        }

        public async ValueTask<IEnumerable<MaterializedWorkflow>> GetWorkflowsAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return new List<MaterializedWorkflow>();
        }

        public string Name => "KONTECG";

        private async Task<MaterializedWorkflow> BuildWorkflowDefinitionAsync(Func<IIocResolver, ValueTask<IWorkflow>> workflowFactory, CancellationToken cancellationToken)
        {
            var builder = _workflowBuilderFactory.CreateBuilder();
            var workflowBuilder = await workflowFactory(_iocResolver);
            var workflowBuilderType = workflowBuilder.GetType();

            builder.DefinitionId = workflowBuilderType.Name;
            await workflowBuilder.BuildAsync(builder, cancellationToken);

            var workflow = await builder.BuildWorkflowAsync(cancellationToken);
            var materializeContext = new KontecgWorkflowMaterializerContext(workflowBuilder.GetType());
            return new MaterializedWorkflow(workflow, Name,KontecgWorkflowMaterializer.MaterializerName, materializeContext);
        }
    }
}
