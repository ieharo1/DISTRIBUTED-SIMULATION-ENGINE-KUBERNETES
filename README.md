# ☸️ Distributed Simulation Engine

Motor distribuido de simulaciones matematicas con API en C# y workers en Kubernetes.

---

## ✅ Descripcion

El API crea tareas de simulacion en Redis y varios workers ejecutan los calculos en paralelo.

### ¿Que hace este proyecto?

- **API .NET**: Crea tareas de simulacion
- **Redis Queue**: Cola de trabajos
- **Workers**: Ejecutan Monte Carlo, estimacion de PI y probabilidades

---

## ✨ Caracteristicas Principales

| Caracteristica | Descripcion |
|----------------|-------------|
| **Simulaciones** | Monte Carlo, PI, Probabilidades |
| **Cola distribuida** | Redis como buffer |
| **Escalado** | Multiples workers |
| **Kubernetes** | Manifests listos |

---

## 🛠️ Stack Tecnologico

- **C# / .NET 8**
- **Redis 7**
- **Docker / Kubernetes**

---

## 📦 Instalacion y Uso

### Probar con Kubernetes

1) Construir imagenes:

```bash
docker build -t simulation-api:latest -f docker/api.Dockerfile .
docker build -t simulation-worker:latest -f docker/worker.Dockerfile .
```

2) Aplicar manifests:

```bash
kubectl apply -f k8s/redis.yaml
kubectl apply -f k8s/api-deployment.yaml
kubectl apply -f k8s/worker-deployment.yaml
```

3) Crear tarea:

```bash
curl -X POST "http://localhost:30082/simulate?type=pi&iterations=100000"
```

---

## 🗂️ Estructura del Proyecto

```
distributed-simulation-engine
├── api
├── workers
├── simulations
├── k8s
├── docker
└── README.md
```

---

© 2026 Isaac Esteban Haro Torres - Todos los derechos reservados.

