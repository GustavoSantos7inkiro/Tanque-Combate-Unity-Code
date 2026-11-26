using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPiece : MonoBehaviour

{
    public GameObject explosionPrefab; // Prefab da explosão
    private Animator animator; // Referência ao Animator do bloco de chão

    void Start()
    {
        // Inicializa o Animator
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se a colisão é com a bala
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Ativa o Trigger de explosão
            animator.SetTrigger("Explode");

            // Instancia a explosão na posição da colisão
            TriggerExplosion(collision.contacts[0].point);
            
            // Destroi ou desativa o bloco de chão após a explosão
            DestroyPartOfGround();
        }
    }

    void TriggerExplosion(Vector2 position)
    {
        // Instancia a explosão no ponto da colisão
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
            Destroy(explosion, 0.5f); // Destroi a explosão após a animação
        }
    }

    void DestroyPartOfGround()
    {
        // Desativa o bloco de chão após a explosão
        gameObject.SetActive(false);
        
    }
}
