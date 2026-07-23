# User Mods

Coloque aqui pastas de mods da comunidade. Cada mod é uma pasta com:

```
meu_mod/
├── mod.json      # obrigatório (metadados)
└── units.json    # opcional (unidades novas ou overrides)
```

## Regras

1. O `base_mod` carrega primeiro.
2. Pastas em `user_mods/` carregam em ordem alfabética.
3. Se `override_base` for `true` no `mod.json`, unidades com o mesmo `id` substituem as do base.
4. Pastas que começam com `_` ou `.` são ignoradas.

## Exemplo rápido

Veja `example_hostile_blob/` nesta pasta.

Documentação completa: [docs/MODDING.md](../../docs/MODDING.md)
