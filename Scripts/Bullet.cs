using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class Bullet : MonoBehaviour
{
    public GameObject explosionPrefab; // Prefab da explosão
    public GameObject shooterTank;    // Referência ao tanque que disparou a bala

    // Variáveis para os sons
    
    
    public AudioClip shootSound;       // Som de disparo
    public AudioSource audioSource;    // Referência ao AudioSource

    public float damage = 10f; // Quantidade de dano que a bala causa

    void Start()
    {
        // Certificar-se de que o AudioSource está ativado
        if (audioSource != null && !audioSource.enabled)
        {
            Debug.Log("AudioSource estava desativado. Ativando...");
            audioSource.enabled = true;
        }

        // Obtém o AudioSource do objeto da bala
        audioSource = GetComponent<AudioSource>();

        // Toca o som de disparo assim que a bala for criada
        PlayShootSound();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se a colisão é com o chão
        if (collision.gameObject.CompareTag("Ground"))
        {
            TriggerExplosion(collision.contacts[0].point); // Ponto de colisão
            DestroyGroundBlock(collision.gameObject); // Destroi o bloco de chão
            
            Destroy(gameObject); // A bala desaparece imediatamente após a colisão
        }
        // Verifica se a colisão é com o Tilemap Bricks
        else if (collision.gameObject.CompareTag("Bricks"))
        {
            TriggerExplosion(collision.contacts[0].point); // Ponto de colisão
            
            Destroy(gameObject); // A bala desaparece imediatamente após a colisão
        }
        // Verifica se a colisão é com o tanque 1 (não é o tanque que disparou)
        else if (collision.gameObject.CompareTag("ShooterTank1") && collision.gameObject != shooterTank)
        {
            Debug.Log("A bala atingiu o Tank 1!");
            TriggerExplosion(collision.contacts[0].point); // Ponto de colisão
            

            // Aplica dano ao Tank1
            var tank1Controller = collision.gameObject.GetComponent<TankController>();
            if (tank1Controller != null)
            {
                tank1Controller.TakeDamage(damage); // Aplica o dano ao Tank1
            }

            Destroy(gameObject); // A bala desaparece
        }
        // Verifica se a colisão é com o tanque 2 (não é o tanque que disparou)
        else if (collision.gameObject.CompareTag("ShooterTank2") && collision.gameObject != shooterTank)
        {
            Debug.Log("A bala atingiu o Tank 2!");
            TriggerExplosion(collision.contacts[0].point); // Ponto de colisão
            

            // Aplica dano ao Tank2
            var tank2Controller = collision.gameObject.GetComponent<TankController>();
            if (tank2Controller != null)
            {
                tank2Controller.TakeDamage(damage); // Aplica o dano ao Tank2
            }

            Destroy(gameObject); // A bala desaparece
        }
    }

    void TriggerExplosion(Vector2 position)
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
            Destroy(explosion, 0.5f); // Destroi a explosão após a animação
        }
    }

    void DestroyGroundBlock(GameObject groundBlock)
    {
        Animator groundAnimator = groundBlock.GetComponent<Animator>();
        if (groundAnimator != null)
        {
            groundAnimator.SetTrigger("Explode");
        }

        Destroy(groundBlock); // Destrói o bloco de chão imediatamente
    }

    
    

    // Função para tocar o som de impacto
        void PlayShootSound()
    {
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound); // Toca o som de disparo
        }
    }
}