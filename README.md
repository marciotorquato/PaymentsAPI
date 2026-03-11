# 💳 PaymentsAPI — FCG FIAP Cloud Games

Microsserviço responsável pelo **processamento de pagamentos** da plataforma FIAP Cloud Games. Não expõe endpoints HTTP — opera exclusivamente via eventos RabbitMQ.

---

## 🧱 Tecnologias

- .NET 9
- SQL Server (dados relacionais)
- MongoDB (logs via Serilog)
- RabbitMQ (consumo e publicação de eventos)

---

## ⚡ Como Funciona

A PaymentsAPI é um serviço orientado a eventos. Ela não possui endpoints HTTP — todo o seu fluxo de trabalho é acionado por mensagens do RabbitMQ.

```
CatalogAPI
  └── publica → order-placed-exchange
        └── PaymentsAPI consome → processa pagamento
              └── publica → payment-processed-exchange
                    ├── CatalogAPI consome → libera jogo na biblioteca
                    └── NotificationsAPI consome → notifica o usuário
```

---

## 📨 Eventos

### Consumidos

| Exchange | Fila | Ação |
|---|---|---|
| `order-placed-exchange` | `order-placed-queue-payments` | Recebe pedido de compra e processa o pagamento |

### Publicados

| Exchange | Tipo | Filas vinculadas | Quando |
|---|---|---|---|
| `payment-processed-exchange` | Fanout | `payment-processed-queue-catalog` `payment-processed-queue-notifications` | Após processar o pagamento (aprovado ou rejeitado) |

---

## 🔄 Fluxo de Processamento

1. Recebe evento `OrderPlacedEvent` da CatalogAPI
2. Processa o pagamento (aprovado ou rejeitado)
3. Publica evento `PaymentProcessedEvent` com o resultado
4. CatalogAPI e NotificationsAPI consomem o resultado e tomam as ações cabíveis

---

## 🗃️ Banco de Dados

| Configuração | Valor |
|---|---|
| Connection String | `MS_PaymentAPI` |
| Database | `MS_PaymentAPI` |

---

## 🐳 Rodando Localmente (Docker Compose)

Este serviço faz parte da orquestração central. Para rodar o ambiente completo:

```bash
# Clone todos os repositórios na mesma pasta pai
git clone https://github.com/pablosdlima/OrchestrationApi
git clone https://github.com/marciotorquato/PaymentsAPI

# Suba o ambiente
cd OrchestrationAPI
docker compose up --build
```

> ℹ️ Este serviço não possui Swagger pois não expõe endpoints HTTP. Para monitorar os eventos, acesse o painel do RabbitMQ em `http://localhost:15672` com as credenciais `admin` / `admin`.

---

## ☸️ Rodando com Kubernetes

### Pré-requisitos

- Docker Desktop com **Kubernetes habilitado**
- `kubectl` disponível no terminal
- Infraestrutura já aplicada via OrchestrationAPI

### Estrutura dos manifestos

```
PaymentsAPI/
└── k8s/
    ├── configmap.yaml   ← variáveis não sensíveis
    ├── secret.yaml      ← variáveis sensíveis (Base64)
    ├── deployment.yaml  ← gerencia os Pods
    └── service.yaml     ← expõe o serviço na rede
```

### 1. Aplicar os manifestos

```bash
# Na raiz do repositório PaymentsAPI
kubectl apply -f k8s/
```

### 2. Verificar se está rodando

```bash
kubectl get pods
```

### 3. Monitorar os eventos

```bash
# Ver logs do serviço em tempo real
kubectl logs -f payments-api-xxx

# Ou acesse o painel do RabbitMQ
http://localhost:30072
```

> ℹ️ Substitua `payments-api-xxx` pelo nome real do Pod obtido via `kubectl get pods`.

### Parar o serviço

```bash
kubectl delete -f k8s/
```

---

## 🎓 Contexto Acadêmico

Desenvolvido para o **Tech Challenge Fase 2 — PosTech FIAP**
Arquitetura de Software em .NET com Azure.