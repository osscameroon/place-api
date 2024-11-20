using System.Collections.Generic;

namespace Common.Mediatr.Behaviours.Logging;

// Interface de base pour marquer les objets loggables
public interface ILoggable
{
    IDictionary<string, object?> ToLog();
}

// Helper statique pour la logique de logging commune

// Pipeline behavior amélioré

// Interface pour enrichir les logs avec des données supplémentaires
