1. When deploying frontend, its network must be declared beforehand (maybe we can force-create)
    * If forgotten: deployment fails

Reconciliation loop:
2. We must connect Nginx to this new network proxy-net-{customer}-{application}-{component}
    * If forgotten: customer's application's webpage doesn't show
3. We must restart Nginx after applying new network config - this seems like a new thing
4. If nginx ever gets redeployed its networks are wiped 
    * If forgotten: total blackout
