using System;
using System.Collections.Generic;
using SimpleServer.Exceptions;
using SimpleServer.Networking;

namespace SimpleServer.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RestController : Attribute
    {
        public static List<Type> Controllers { get; private set; } = new List<Type>();

        public string BasePath { get; set; }

        public RestController(string basePath)
        {
            BasePath = basePath;
        }

        /// <summary>
        /// Extract all method mappings from specified type
        /// </summary>
        /// <param name="controller">controller should contain the RestController attribute</param>
        internal static void ExtractRequestMethods(Type controller)
        {
            Controllers.Add(controller);
            foreach (var method in controller.GetMethods())
            {
                // If we have more than one AbstractMapping attribute on a method, throw an error
                if (method.GetCustomAttributes(typeof(AbstractMapping), true).Length > 1)
                {
                    throw new ServerMappingException("Found more than one Mapping on a method. Methods only allow one mapping attribute");
                }

                foreach (var attr in method.GetCustomAttributes(typeof(GetMapping), true))
                {
                    GetMapping getAttr = (GetMapping)attr;
                    AbstractMapping.Mapping[HttpMethod.GET].Add(getAttr.Path, new MappingInfo<AbstractMapping>(getAttr, method, controller));

                }
                foreach (var attr in method.GetCustomAttributes(typeof(PostMapping), true))
                {
                    PostMapping postAttr = (PostMapping)attr;
                    AbstractMapping.Mapping[HttpMethod.POST].Add(postAttr.Path, new MappingInfo<AbstractMapping>(postAttr, method, controller));
                }
                foreach (var attr in method.GetCustomAttributes(typeof(DeleteMapping), true))
                {
                    DeleteMapping deleteAttr = (DeleteMapping)attr;
                    AbstractMapping.Mapping[HttpMethod.DELETE].Add(deleteAttr.Path, new MappingInfo<AbstractMapping>(deleteAttr, method, controller));
                }
                foreach (var attr in method.GetCustomAttributes(typeof(PutMapping), true))
                {
                    PutMapping putAttr = (PutMapping)attr;
                    AbstractMapping.Mapping[HttpMethod.PUT].Add(putAttr.Path, new MappingInfo<AbstractMapping>(putAttr, method, controller));
                }
            }
        }
    }
}